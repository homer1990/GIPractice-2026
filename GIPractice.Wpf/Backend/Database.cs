using GIPractice.Api.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

public sealed class Database : IDatabase
{
    private readonly HttpClient _httpClient;
    private readonly BackendContext _context;
    private LoginResponseDto? _loginResponse;
    private ConnectionState _connectionState = ConnectionState.Disconnected;
    private readonly IClientSettings _settings;
    public CurrentUserDto? CurrentUser => _loginResponse?.User;

    public bool IsAuthenticated =>
        _loginResponse is { AccessToken.Length: > 0 } r &&
        r.ExpiresAtUtc > DateTime.UtcNow;
    public string ServerUrl => _settings.ServerUrl;
    public Database(IClientSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        _httpClient = new HttpClient();
        _context = new BackendContext(_httpClient);

        ApplyServerUrl();
    }
    public void SetServerUrl(string serverUrl)
    {
        if (string.IsNullOrWhiteSpace(serverUrl))
            throw new ArgumentException("Server URL cannot be empty.", nameof(serverUrl));

        _settings.ServerUrl = serverUrl;
        _settings.Save();
        ApplyServerUrl();

        // Reset auth/session because server changed
        _loginResponse = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        ConnectionState = ConnectionState.Disconnected;
    }

    private void ApplyServerUrl()
    {
        if (Uri.TryCreate(_settings.ServerUrl, UriKind.Absolute, out var uri))
        {
            _httpClient.BaseAddress = uri;
        }
        else
        {
            _httpClient.BaseAddress = null;
        }
    }

    public ConnectionState ConnectionState
    {
        get => _connectionState;
        private set
        {
            if (_connectionState == value) return;
            var old = _connectionState;
            _connectionState = value;
            ConnectionStateChanged?.Invoke(
                this,
                new ConnectionStateChangedEventArgs(old, _connectionState));
        }
    }

    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;
    public event EventHandler<SessionEndedEventArgs>? SessionEnded;

    public async Task<TResult> QueryAsync<TResult>(
        IBackendQuery<TResult> query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var ok = await EnsureConnectedAsync(cancellationToken);
        if (!ok)
            throw new InvalidOperationException("Not authenticated. Please login first.");

        return await query.ExecuteAsync(_context, cancellationToken);
    }

    public Task<bool> EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        if (IsAuthenticated)
            return Task.FromResult(true);

        if (_loginResponse is not null &&
            _loginResponse.ExpiresAtUtc <= DateTime.UtcNow)
        {
            OnSessionEnded(SessionEndedReason.TokenExpired, "Session expired. Please login again.");
        }

        return Task.FromResult(false);
    }


    public void RegisterUserInteraction()
    {
        // later: inactivity timer
    }

    private void OnSessionEnded(SessionEndedReason reason, string? message = null)
    {
        ConnectionState = ConnectionState.Disconnected;
        SessionEnded?.Invoke(this, new SessionEndedEventArgs(reason, message));
    }
    public async Task<bool> LoginAsync(
    string userName,
    string password,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username is required.", nameof(userName));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        var request = new LoginRequestDto
        {
            UserName = userName,
            Password = password
        };

        ConnectionState = ConnectionState.Connecting;

        using var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",              // AuthController: POST /api/auth/login
            request,
            cancellationToken);

        if (response.StatusCode is System.Net.HttpStatusCode.BadRequest
                               or System.Net.HttpStatusCode.Unauthorized)
        {
            // Invalid credentials
            ConnectionState = ConnectionState.Disconnected;
            return false;
        }

        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<LoginResponseDto>(
            cancellationToken: cancellationToken);

        if (login is null || string.IsNullOrWhiteSpace(login.AccessToken))
        {
            ConnectionState = ConnectionState.Disconnected;
            throw new InvalidOperationException("Login response did not contain an access token.");
        }

        _loginResponse = login;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);

        ConnectionState = ConnectionState.Connected;
        return true;
    }
    public Task LogoutAsync()
    {
        _loginResponse = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        OnSessionEnded(SessionEndedReason.ExplicitLogout, "User logged out.");
        return Task.CompletedTask;
    }

}

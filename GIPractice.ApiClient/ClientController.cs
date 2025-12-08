using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public class ClientController : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly SynchronizationContext? _syncContext;
    private readonly object _sync = new();
    private readonly CancellationTokenSource _monitorCts = new();
    private readonly TimeSpan _refreshSkew = TimeSpan.FromMinutes(2);
    private readonly TimeSpan _monitorInterval = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _inactivityTimeout = TimeSpan.FromMinutes(30);

    private DateTime _lastActivityUtc = DateTime.UtcNow;
    private LoginResponseDto? _session;
    private string? _cachedUserName;
    private string? _cachedPassword;
    private bool _isOnline = true;
    private bool _isRefreshing;

    public ClientController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _syncContext = SynchronizationContext.Current;

        _ = Task.Run(async () => await MonitorAsync(_monitorCts.Token));
    }

    public bool IsOnline
    {
        get => _isOnline;
        private set
        {
            if (_isOnline == value) return;
            _isOnline = value;
            if (_isOnline)
                RaiseOnUi(ConnectivityRestored);
            else
                RaiseOnUi(ConnectivityLost);
        }
    }

    public bool IsAuthenticated => _session != null;
    public LoginResponseDto? CurrentSession => _session;

    public event EventHandler? ConnectivityLost;
    public event EventHandler? ConnectivityRestored;
    public event EventHandler? InactivityTimedOut;
    public event EventHandler? TokenExpiring;
    public event EventHandler? TokenExpired;
    public event EventHandler? TokenRefreshed;
    public event EventHandler<LoginResponseDto>? LoggedIn;
    public event EventHandler? LoggedOut;

    public Task<LoginResponseDto?> LoginAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default) =>
        LoginInternalAsync(userName, password, isRefresh: false, cancellationToken);

    private async Task<LoginResponseDto?> LoginInternalAsync(
        string userName,
        string password,
        bool isRefresh,
        CancellationToken cancellationToken = default)
    {
        var request = new LoginRequestDto
        {
            UserName = userName,
            Password = password
        };

        return await ExecuteAsync(async () =>
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            response.EnsureSuccessStatusCode();

            var login = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);
            if (login?.AccessToken == null) return null;

            ApplySession(login, userName, password, isRefresh);
            return login;
        }, cancellationToken);
    }

    public Task LogoutAsync()
    {
        lock (_sync)
        {
            _session = null;
            _cachedUserName = null;
            _cachedPassword = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        RaiseOnUi(LoggedOut);
        return Task.CompletedTask;
    }

    public async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return default;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            return result;
        }, cancellationToken);
    }

    private void ApplySession(LoginResponseDto login, string userName, string password, bool isRefresh)
    {
        lock (_sync)
        {
            _session = login;
            _cachedUserName = userName;
            _cachedPassword = password;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", login.AccessToken);
            _lastActivityUtc = DateTime.UtcNow;
        }

        if (isRefresh)
            RaiseOnUi(TokenRefreshed);
        else
            RaiseOnUi(() => LoggedIn?.Invoke(this, login));
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(_monitorInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await CheckInactivityAsync();
                await CheckTokenExpiryAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
        finally
        {
            timer.Dispose();
        }
    }

    private Task CheckInactivityAsync()
    {
        if (_session == null) return Task.CompletedTask;

        var idle = DateTime.UtcNow - _lastActivityUtc;
        if (idle > _inactivityTimeout)
        {
            RaiseOnUi(InactivityTimedOut);
            return LogoutAsync();
        }

        return Task.CompletedTask;
    }

    private async Task CheckTokenExpiryAsync()
    {
        if (_session == null) return;

        var expires = _session.ExpiresAtUtc;
        var now = DateTime.UtcNow;
        if (expires <= now)
        {
            RaiseOnUi(TokenExpired);
            await LogoutAsync();
            return;
        }

        var remaining = expires - now;
        if (remaining <= _refreshSkew)
        {
            RaiseOnUi(TokenExpiring);
            await RefreshTokenAsync();
        }
    }

    private async Task<bool> RefreshTokenAsync()
    {
        if (_isRefreshing) return true;

        string? user;
        string? pass;
        lock (_sync)
        {
            user = _cachedUserName;
            pass = _cachedPassword;
            _isRefreshing = true;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                return false;

            var refreshed = await LoginInternalAsync(user, pass, isRefresh: true);
            return refreshed != null;
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private async Task HandleUnauthorizedAsync()
    {
        RaiseOnUi(TokenExpired);
        await LogoutAsync();
    }

    private async Task<T?> ExecuteAsync<T>(Func<Task<T?>> action, CancellationToken cancellationToken)
    {
        try
        {
            var result = await action();
            RecordActivity();
            IsOnline = true;
            return result;
        }
        catch (HttpRequestException)
        {
            IsOnline = false;
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            IsOnline = false;
            throw new HttpRequestException("Request timed out", ex);
        }
    }

    private void RecordActivity()
    {
        lock (_sync)
        {
            _lastActivityUtc = DateTime.UtcNow;
        }
    }

    private void RaiseOnUi(EventHandler? handler)
    {
        if (handler == null) return;
        void Raise() => handler(this, EventArgs.Empty);
        if (_syncContext != null)
            _syncContext.Post(_ => Raise(), null);
        else
            Raise();
    }

    private void RaiseOnUi(Action raise)
    {
        if (_syncContext != null)
            _syncContext.Post(_ => raise(), null);
        else
            raise();
    }

    public void Dispose()
    {
        _monitorCts.Cancel();
        _monitorCts.Dispose();
    }
}

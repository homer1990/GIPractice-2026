using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using GIPractice.Client;
using Microsoft.Extensions.Logging;

namespace GIPractice.Client;

public sealed class Database : IDatabaseController, IAsyncDisposable
{
    private readonly DatabaseOptions _options;
    private readonly ITokenService _tokenService;
    private readonly ILogger<Database> _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly PeriodicTimer _monitorTimer;
    private readonly SemaphoreSlim _clientLock = new(1, 1);
    
    private HttpClient _httpClient;
    private ConnectionStatus _status = ConnectionStatus.Connecting;
    private DateTimeOffset _lastActivity;

    public Database(HttpClient httpClient, DatabaseOptions options, ITokenService tokenService, ILogger<Database> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _lastActivity = DateTimeOffset.UtcNow;

        _httpClient.BaseAddress = NormalizeBaseAddress(options.BaseAddress);
        _monitorTimer = new PeriodicTimer(_options.ConnectivityCheckInterval);
        _ = MonitorConnectivityAsync();
    }

    public event EventHandler<ConnectionStateChange>? StatusChanged;
    public event EventHandler<InterruptSignal>? InterruptRaised;

    public void RecordActivity()
    {
        _lastActivity = DateTimeOffset.UtcNow;
    }

    public async Task<IReadOnlyList<TDto>> QueryAsync<TQuery, TDto>(string route, TQuery query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RecordActivity();

        if (!await EnsureAuthorizedAsync(cancellationToken).ConfigureAwait(false))
        {
            return [];
        }

        await _clientLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(route, query, cancellationToken).ConfigureAwait(false);

            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => HandleUnauthorized<TDto>(route),
                HttpStatusCode.ServiceUnavailable => HandleServiceUnavailable<TDto>(),
                _ => await HandleSuccessResponse<TDto>(response, cancellationToken).ConfigureAwait(false)
            };
        }
        finally
        {
            _clientLock.Release();
        }
    }

    private async Task<bool> EnsureAuthorizedAsync(CancellationToken cancellationToken)
    {
        if (!await _tokenService.EnsureValidTokenAsync(cancellationToken).ConfigureAwait(false))
        {
            RaiseInterrupt(new InterruptSignal(InterruptReason.Unauthorized, "Cannot refresh token. Please sign in again."));
            UpdateStatus(ConnectionStatus.Unauthorized, "Authorization required");
            return false;
        }

        ApplyAuthorizationHeader();

        if (_status != ConnectionStatus.Connected)
        {
            UpdateStatus(ConnectionStatus.Connecting, "Connecting to API");
            var isHealthy = await ProbeHealthAsync(cancellationToken).ConfigureAwait(false);
            UpdateStatus(isHealthy ? ConnectionStatus.Connected : ConnectionStatus.Disconnected,
                isHealthy ? "Connected" : "Disconnected");
        }

        return true;
    }

    private void ApplyAuthorizationHeader()
    {
        if (!string.IsNullOrWhiteSpace(_tokenService.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenService.AccessToken);
        }
    }

    private async Task MonitorConnectivityAsync()
    {
        try
        {
            while (await _monitorTimer.WaitForNextTickAsync(_cts.Token).ConfigureAwait(false))
            {
                var now = DateTimeOffset.UtcNow;

                if (now - _lastActivity > _options.InactivityTimeout)
                {
                    RaiseInterrupt(new InterruptSignal(InterruptReason.Inactivity, "Session inactive"));
                    await _tokenService.SignOutAsync().ConfigureAwait(false);
                    UpdateStatus(ConnectionStatus.Unauthorized, "Signed out due to inactivity");
                    continue;
                }

                await CheckConnectivityAsync(_cts.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on disposal
        }
    }

    private async Task CheckConnectivityAsync(CancellationToken cancellationToken)
    {
        await _clientLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!await _tokenService.EnsureValidTokenAsync(cancellationToken).ConfigureAwait(false))
            {
                UpdateStatus(ConnectionStatus.Unauthorized, "Token refresh failed");
                return;
            }

            ApplyAuthorizationHeader();
            var healthy = await ProbeHealthAsync(cancellationToken).ConfigureAwait(false);
            var newStatus = healthy ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;

            if (_status != newStatus)
            {
                UpdateStatus(newStatus, healthy ? "Connected" : "Connection lost");

                if (!healthy)
                {
                    RaiseInterrupt(new InterruptSignal(InterruptReason.ConnectivityLost, "Connection lost, retrying..."));
                }
            }
        }
        finally
        {
            _clientLock.Release();
        }
    }

    private async Task<bool> ProbeHealthAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetAsync(_options.HealthEndpoint, cancellationToken).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Health probe failed");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error during health probe");
            return false;
        }
    }

    private IReadOnlyList<TDto> HandleUnauthorized<TDto>(string route)
    {
        _logger.LogWarning("Unauthorized response received from {Route}", route);
        _ = _tokenService.SignOutAsync();
        RaiseInterrupt(new InterruptSignal(InterruptReason.TokenExpired, "Session expired. Please sign in again."));
        UpdateStatus(ConnectionStatus.Unauthorized, "Unauthorized");
        return [];
    }

    private IReadOnlyList<TDto> HandleServiceUnavailable<TDto>()
    {
        UpdateStatus(ConnectionStatus.Disconnected, "API unavailable");
        RaiseInterrupt(new InterruptSignal(InterruptReason.ConnectivityLost, "Connection interrupted while executing query."));
        return [];
    }

    private async Task<IReadOnlyList<TDto>> HandleSuccessResponse<TDto>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<TDto>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        UpdateStatus(ConnectionStatus.Connected, "Data received");
        return payload ?? [];
    }

    private void RaiseInterrupt(InterruptSignal signal)
    {
        InterruptRaised?.Invoke(this, signal);
    }

    private void UpdateStatus(ConnectionStatus status, string? message = null)
    {
        if (_status == status && message is null)
        {
            return;
        }

        _status = status;
        StatusChanged?.Invoke(this, new ConnectionStateChange(status, message));
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _monitorTimer.Dispose();
        _cts.Dispose();
        _clientLock.Dispose();
        _httpClient.Dispose();
        if (_tokenService is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task ApplyClientSettingsAsync(ClientSettings settings)
    {
        await _clientLock.WaitAsync().ConfigureAwait(false);
        try
        {
            var baseAddress = NormalizeBaseAddress(settings.ApiBaseAddress);
            _options.BaseAddress = baseAddress;
            _options.HealthEndpoint = settings.HealthEndpoint;

            var oldClient = _httpClient;
            _httpClient = new HttpClient { BaseAddress = baseAddress };
            oldClient.Dispose();

            UpdateStatus(ConnectionStatus.Connecting, "Connecting to API");
        }
        finally
        {
            _clientLock.Release();
        }
    }

    private static Uri NormalizeBaseAddress(string baseAddress)
    {
        var uri = new Uri(baseAddress, UriKind.Absolute);
        if (!uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
        {
            uri = new Uri(uri.AbsoluteUri + "/");
        }
        return uri;
    }

    private static Uri NormalizeBaseAddress(Uri baseAddress)
    {
        var uri = baseAddress;
        if (!uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
        {
            uri = new Uri(uri.AbsoluteUri + "/");
        }
        return uri;
    }
}

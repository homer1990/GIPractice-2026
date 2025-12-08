using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Client.Core;
using Microsoft.Extensions.Logging;

namespace GIPractice.ApiClient;

public sealed class Database : IDatabaseController, IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly DatabaseOptions _options;
    private readonly ITokenService _tokenService;
    private readonly ILogger<Database> _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly PeriodicTimer _monitorTimer;
    private ConnectionStatus _status = ConnectionStatus.Connecting;
    private DateTimeOffset _lastActivity;

    public Database(HttpClient httpClient, DatabaseOptions options, ITokenService tokenService, ILogger<Database> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _tokenService = tokenService;
        _logger = logger;
        _lastActivity = DateTimeOffset.UtcNow;

        _httpClient.BaseAddress = options.BaseAddress;
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

        await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

        using var response = await _httpClient.PostAsJsonAsync(route, query, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Unauthorized response received from {Route}", route);
            await _tokenService.SignOutAsync().ConfigureAwait(false);
            RaiseInterrupt(new InterruptSignal(InterruptReason.TokenExpired, "Session expired. Please sign in again."));
            UpdateStatus(ConnectionStatus.Unauthorized, "Unauthorized");
            return Array.Empty<TDto>();
        }

        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            UpdateStatus(ConnectionStatus.Disconnected, "API unavailable");
            RaiseInterrupt(new InterruptSignal(InterruptReason.ConnectivityLost, "Connection interrupted while executing query."));
            return Array.Empty<TDto>();
        }

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<TDto>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        UpdateStatus(ConnectionStatus.Connected, "Data received");
        return payload ?? Array.Empty<TDto>();
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (!await _tokenService.EnsureValidTokenAsync(cancellationToken).ConfigureAwait(false))
        {
            RaiseInterrupt(new InterruptSignal(InterruptReason.Unauthorized, "Cannot refresh token. Please sign in again."));
            UpdateStatus(ConnectionStatus.Unauthorized, "Authorization required");
            return;
        }

        if (!string.IsNullOrWhiteSpace(_tokenService.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenService.AccessToken);
        }

        if (_status == ConnectionStatus.Connected)
        {
            return;
        }

        UpdateStatus(ConnectionStatus.Connecting, "Connecting to API");
        var isHealthy = await ProbeHealthAsync(cancellationToken).ConfigureAwait(false);
        UpdateStatus(isHealthy ? ConnectionStatus.Connected : ConnectionStatus.Disconnected, isHealthy ? "Connected" : "Disconnected");
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

                var healthy = await ProbeHealthAsync(_cts.Token).ConfigureAwait(false);
                var newStatus = healthy ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
                UpdateStatus(newStatus, healthy ? "Connected" : "Connection lost");

                if (!healthy)
                {
                    RaiseInterrupt(new InterruptSignal(InterruptReason.ConnectivityLost, "Connection lost, retrying..."));
                }
            }
        }
        catch (OperationCanceledException)
        {
            // expected on disposal
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
        _httpClient.Dispose();
        if (_tokenService is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
    }
}

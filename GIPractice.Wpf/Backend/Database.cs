using System;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

/// <summary>
/// Single controller / gateway between UI and backend.
/// Will later hold HttpClient, JWT, inactivity timers, etc.
/// </summary>
public sealed class Database : IDatabase
{
    private ConnectionState _connectionState = ConnectionState.Disconnected;

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

        // Later:
        //  - EnsureConnectedAsync()
        //  - central exception handling
        //  - token renewal / connectivity checks
        //  - map HTTP errors to SessionEnded, etc.

        await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

        // For now, this will throw until we implement real queries.
        throw new NotImplementedException(
            "QueryAsync is not wired to the HTTP API yet. " +
            "We will implement this once the API endpoints are finalised.");
    }

    public Task<bool> EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        // For now we just flip state; later we will ping the API / login.
        if (ConnectionState == ConnectionState.Disconnected)
        {
            ConnectionState = ConnectionState.Connecting;
            ConnectionState = ConnectionState.Connected;
        }

        return Task.FromResult(true);
    }

    public void RegisterUserInteraction()
    {
        // Later:
        //  - reset inactivity timer
        //  - if timer fires -> OnSessionEnded(SessionEndedReason.Inactivity, ...)
    }

    private void OnSessionEnded(SessionEndedReason reason, string? message = null)
    {
        ConnectionState = ConnectionState.Disconnected;
        SessionEnded?.Invoke(this, new SessionEndedEventArgs(reason, message));
    }
}

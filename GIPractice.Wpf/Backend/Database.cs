using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

/// <summary>
/// Single controller / gateway between UI and backend.
/// Holds HttpClient, connection state, inactivity, etc.
/// </summary>
public sealed class Database : IDatabase
{
    private readonly HttpClient _httpClient;
    private readonly BackendContext _context;
    private ConnectionState _connectionState = ConnectionState.Disconnected;

    public Database()
    {
        // TODO: move base address + default headers to config.
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5001") // <-- adjust to your API
        };

        _context = new BackendContext(_httpClient);
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
        if (query is null) throw new ArgumentNullException(nameof(query));

        await EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

        // Later:
        //  - central exception handling
        //  - JWT renewal / 401 handling -> SessionEnded
        //  - connectivity retry logic

        return await query.ExecuteAsync(_context, cancellationToken)
                          .ConfigureAwait(false);
    }

    public Task<bool> EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        // Stub for now. Later: ping /health, ensure token, etc.
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

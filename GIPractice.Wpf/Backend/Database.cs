using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

public sealed class Database : IDatabase
{
    private readonly HttpClient _httpClient;
    private readonly BackendContext _context;
    private ConnectionState _connectionState = ConnectionState.Disconnected;

    public Database()
    {
        // TODO: adjust base address to your API
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5001") // <- change if needed
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

        await EnsureConnectedAsync(cancellationToken);

        // later: error handling, JWT, 401 → SessionEnded, etc.
        return await query.ExecuteAsync(_context, cancellationToken);
    }

    public Task<bool> EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        if (ConnectionState == ConnectionState.Disconnected)
        {
            ConnectionState = ConnectionState.Connecting;
            // TODO: ping /health, login/token, etc.
            ConnectionState = ConnectionState.Connected;
        }

        return Task.FromResult(true);
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
}

using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

public interface IDatabase
{
    ConnectionState ConnectionState { get; }

    event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;
    event EventHandler<SessionEndedEventArgs>? SessionEnded;

    /// <summary>
    /// Execute a backend query (API call) and return DTO result.
    /// Concrete IBackendQuery implementations will know which endpoint to call.
    /// </summary>
    Task<TResult> QueryAsync<TResult>(
        IBackendQuery<TResult> query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures that we are connected / logged in.
    /// Later: JWT, health checks, etc.
    /// </summary>
    Task<bool> EnsureConnectedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Called by UI whenever user interacts, for inactivity timer.
    /// </summary>
    void RegisterUserInteraction();
}

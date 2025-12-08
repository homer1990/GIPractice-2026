using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Client;

public enum ConnectionStatus
{
    Connecting,
    Connected,
    Disconnected,
    Unauthorized
}

public enum InterruptReason
{
    ConnectivityLost,
    Inactivity,
    TokenExpired,
    Unauthorized,
    Manual
}

public record ConnectionStateChange(ConnectionStatus Status, string? Message = null);

public record InterruptSignal(InterruptReason Reason, string Message);

public interface IDatabaseController
{
    event EventHandler<ConnectionStateChange>? StatusChanged;
    event EventHandler<InterruptSignal>? InterruptRaised;

    Task<IReadOnlyList<TDto>> QueryAsync<TQuery, TDto>(string route, TQuery query, CancellationToken cancellationToken = default);

    void RecordActivity();
}

public interface ITokenService
{
    string? AccessToken { get; }
    DateTimeOffset? ExpiresAt { get; }
    Task<bool> EnsureValidTokenAsync(CancellationToken cancellationToken);
    Task<bool> SignInAsync(string userName, string password, CancellationToken cancellationToken);
    Task SignOutAsync();
}

public interface IInterruptListener
{
    Task OnInterruptAsync(InterruptSignal signal, CancellationToken cancellationToken);
}

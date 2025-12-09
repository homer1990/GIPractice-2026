using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

/// <summary>
/// Represents a single backend query/command.
/// Database will call ExecuteAsync on this with a BackendContext.
/// </summary>
public interface IBackendQuery<TResult>
{
    Task<TResult> ExecuteAsync(BackendContext context, CancellationToken cancellationToken);
}

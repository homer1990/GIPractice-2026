using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

/// <summary>
/// Represents a single backend query/command.
/// Database will call ExecuteAsync on this.
/// Later it will get an HttpClient or typed API client injected here.
/// </summary>
public interface IBackendQuery<TResult>
{
    Task<TResult> ExecuteAsync(CancellationToken cancellationToken);
}

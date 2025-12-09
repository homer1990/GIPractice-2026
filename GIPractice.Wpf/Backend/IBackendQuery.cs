using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Wpf.Backend;

public interface IBackendQuery<TResult>
{
    Task<TResult> ExecuteAsync(BackendContext context, CancellationToken cancellationToken);
}

using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Client;

public interface ISettingsStore
{
    Task<ClientSettings> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(ClientSettings settings, CancellationToken cancellationToken = default);
}

using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Client;

public interface IPatientsModule
{
    Task<object> SearchAsync(object request, CancellationToken cancellationToken = default);
    Task<object?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(object dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, object dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

// TODO: implement using GiPracticeApiClient like your PatientsModule pattern

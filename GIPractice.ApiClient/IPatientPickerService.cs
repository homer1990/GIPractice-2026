using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public interface IPatientPickerService
{
    Task<PatientListItemDto?> PickPatientAsync(CancellationToken cancellationToken = default);
}

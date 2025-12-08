using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public interface INavigationService
{
    Task ShowShellAsync();

    Task ShowLoginAsync();

    Task ShowSettingsAsync();

    Task ShowPatientDashboardAsync(PatientListItemDto patient);

    Task<PatientListItemDto?> PickPatientAsync();
}

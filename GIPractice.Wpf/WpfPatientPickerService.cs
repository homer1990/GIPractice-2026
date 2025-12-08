using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GIPractice.Api.Models;
using GIPractice.Client;
using Microsoft.Extensions.DependencyInjection;

namespace GIPractice.Wpf;

public class WpfPatientPickerService : IPatientPickerService
{
    private readonly IServiceProvider _services;

    public WpfPatientPickerService(IServiceProvider services)
    {
        _services = services;
    }

    public Task<PatientListItemDto?> PickPatientAsync(CancellationToken cancellationToken = default)
    {
        var window = _services.GetRequiredService<PatientPickerWindow>();
        window.Owner = Application.Current?.MainWindow;

        var result = window.ShowDialog();

        if (result == true && window.SelectedPatient is { } patient)
            return Task.FromResult<PatientListItemDto?>(patient);

        return Task.FromResult<PatientListItemDto?>(null);
    }
}

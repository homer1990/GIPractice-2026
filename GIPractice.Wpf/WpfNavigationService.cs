using System;
using System.Threading.Tasks;
using System.Windows;
using GIPractice.Api.Models;
using GIPractice.Client;
using Microsoft.Extensions.DependencyInjection;

namespace GIPractice.Wpf;

public class WpfNavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    public WpfNavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public Task ShowShellAsync()
    {
        var shell = _services.GetRequiredService<MainWindow>();
        shell.Show();
        Application.Current!.MainWindow = shell;
        return Task.CompletedTask;
    }

    public Task ShowLoginAsync()
    {
        var login = _services.GetRequiredService<LoginWindow>();
        login.Show();
        Application.Current!.MainWindow = login;
        return Task.CompletedTask;
    }

    public async Task ShowSettingsAsync()
    {
        var window = _services.GetRequiredService<SettingsWindow>();
        window.Owner = Application.Current?.MainWindow;
        window.ShowDialog();
        await Task.CompletedTask;
    }

    public async Task ShowPatientDashboardAsync(PatientListItemDto patient)
    {
        var window = _services.GetRequiredService<PatientDashboardWindow>();
        await window.InitializeAsync(patient);
        window.Owner = Application.Current?.MainWindow;
        window.Show();
    }

    public async Task<PatientListItemDto?> PickPatientAsync()
    {
        var window = _services.GetRequiredService<PatientPickerWindow>();
        window.Owner = Application.Current?.MainWindow;
        var result = window.ShowDialog();

        if (result == true)
            return await Task.FromResult(window.SelectedPatient);

        return await Task.FromResult<PatientListItemDto?>(null);
    }
}

using System;
using System.Windows;
using GIPractice.Api.Models;
using GIPractice.Client;
using Microsoft.Extensions.DependencyInjection;

namespace GIPractice.Wpf;

public partial class MainWindow : Window
{
    private readonly ShellViewModel _shell;
    private readonly IServiceProvider _services;

    public MainWindow(ShellViewModel shell, IServiceProvider services)
    {
        InitializeComponent();

        _shell = shell;
        _services = services;

        DataContext = _shell;

        // Hook patient-details event from the patient search VM
        _shell.Patients.OpenPatientDetailsRequested += OnOpenPatientDetailsRequested;
    }

    private async void OnOpenPatientDetailsRequested(object? sender, PatientListItemDto patient)
    {
        var window = _services.GetRequiredService<PatientDashboardWindow>();
        await window.InitializeAsync(patient);
        window.Owner = this;
        window.Show();
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var window = _services.GetRequiredService<SettingsWindow>();
        window.Owner = this;
        window.ShowDialog();
    }

}

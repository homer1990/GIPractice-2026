using GIPractice.Wpf.Backend;
using GIPractice.Wpf.ViewModels;
using GIPractice.Wpf.ViewModels.Patients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace GIPractice.Wpf;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Backend "controller"
        services.AddSingleton<IDatabase, Database>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<PatientSearchViewModel>();

        // Views
        services.AddTransient<Views.Patients.PatientsSearchView>();

        // Windows
        services.AddTransient<MainWindow>();
    }
}

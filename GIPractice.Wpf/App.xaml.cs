using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using GIPractice.Wpf.Backend;
using GIPractice.Wpf.ViewModels;
using GIPractice.Wpf.ViewModels.Auth;

namespace GIPractice.Wpf;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Settings
        services.AddSingleton<IClientSettings>(_ => ClientSettings.Load());

        // Backend controller
        services.AddSingleton<IDatabase, Database>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ViewModels.Patients.PatientSearchViewModel>();
        services.AddTransient<LoginViewModel>();
        // Windows
        services.AddTransient<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (Services is IDisposable d)
            d.Dispose();

        base.OnExit(e);
    }
}

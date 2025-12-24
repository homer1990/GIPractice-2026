using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

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

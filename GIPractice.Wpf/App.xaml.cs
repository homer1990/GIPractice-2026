using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using GIPractice.Wpf.Backend;
using GIPractice.Wpf.ViewModels;

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

        // Windows
        services.AddTransient<MainWindow>();
    }
}

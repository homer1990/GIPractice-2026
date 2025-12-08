using System;
using System.Windows;
using GIPractice.ApiClient;
using GIPractice.Client.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GIPractice.Wpf;

public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var apiSection = context.Configuration.GetSection("Api");
                var clientSection = context.Configuration.GetSection("Client");

                var options = new DatabaseOptions
                {
                    BaseAddress = new Uri(apiSection.GetValue<string>("BaseAddress") ?? "https://localhost:5001/"),
                    HealthEndpoint = apiSection.GetValue<string>("HealthEndpoint") ?? "health",
                    ConnectivityCheckInterval = TimeSpan.FromSeconds(clientSection.GetValue<int>("ConnectivityIntervalSeconds", 10)),
                    InactivityTimeout = TimeSpan.FromMinutes(clientSection.GetValue<int>("InactivityMinutes", 15))
                };

                services.AddSingleton(options);
                services.AddSingleton<ITokenService, InMemoryTokenService>();
                services.AddSingleton<ILocalizationCatalog, JsonLocalizationCatalog>();
                services.AddHttpClient<Database>();
                services.AddSingleton<IDatabaseController>(sp => sp.GetRequiredService<Database>());

                services.AddSingleton<ViewController>();
                services.AddSingleton(sp => new LocalizationManager(
                    sp.GetRequiredService<ILocalizationCatalog>(),
                    clientSection.GetValue<string>("Language") ?? "en"));

                services.AddSingleton<LoginViewModel>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<ShellViewModel>();

                services.AddSingleton<MainWindow>();
            })
            .Build();

        _host.Start();
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}

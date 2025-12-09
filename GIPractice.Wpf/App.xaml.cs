using System;
using System.Windows;
using GIPractice.Client;
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
                var defaultSettings = new ClientSettings
                {
                    ApiBaseAddress = apiSection.GetValue<string>("BaseAddress") ?? "https://localhost:7028/",
                    HealthEndpoint = apiSection.GetValue<string>("HealthEndpoint") ?? "health",
                    InactivityMinutes = clientSection.GetValue("InactivityMinutes", 15),
                    ConnectivityIntervalSeconds = clientSection.GetValue("ConnectivityIntervalSeconds", 10),
                    Language = clientSection.GetValue<string>("Language") ?? "en"
                };

                services.AddSingleton(defaultSettings);
                services.AddSingleton<IClientSettingsStore, RegistrySettingsStore>();
                services.AddSingleton<ClientSettingsManager>();

                services.AddSingleton(sp => 
                {
                    var settingsManager = sp.GetRequiredService<ClientSettingsManager>();
                    return settingsManager.CreateDatabaseOptions();
                });
                services.AddSingleton<ITokenService, InMemoryTokenService>();
                services.AddSingleton<ILocalizationCatalog, JsonLocalizationCatalog>();
                services.AddHttpClient<Database>();
                services.AddSingleton<IDatabaseController>(sp => sp.GetRequiredService<Database>());

                services.AddSingleton<ViewController>();
                services.AddSingleton(sp => new LocalizationManager(
                    sp.GetRequiredService<ILocalizationCatalog>(),
                    sp.GetRequiredService<ClientSettingsManager>().Settings.Language));

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

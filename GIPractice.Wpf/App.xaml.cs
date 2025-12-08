using GIPractice.Client;
using GIPractice.Client.Core;
using GIPractice.Client.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Net.Http;
using System.Windows;

namespace GIPractice.Wpf;

public partial class App : Application
{
    private IHost? _host;
    public static IServiceProvider Services =>
        ((App)Current)._host?.Services ?? _emptyProvider;

    private static readonly IServiceProvider _emptyProvider = new EmptyServiceProvider();

    private class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ISettingsStore, JsonSettingsStore>();
                services.AddSingleton<ClientSettingsService>();

                services.AddSingleton<GiPracticeApiClient>(_ =>
                {
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri("https://localhost:7028/") // match API
                    };
                    return new GiPracticeApiClient(client);
                });

                services.AddSingleton<IStringLocalizer, LocalizationBindingService>();
                services.AddSingleton<IPatientsModule, PatientsModule>();

                services.AddSingleton<ViewController>();

                // UI services
                services.AddTransient<IPatientPickerService, WpfPatientPickerService>();

                // ViewModels
                services.AddSingleton<HomeViewModel>();
                services.AddSingleton<LoginViewModel>();
                services.AddTransient<PatientSearchViewModel>();
                services.AddSingleton<ShellViewModel>();
                services.AddTransient<PatientDashboardViewModel>();
                services.AddSingleton<SettingsViewModel>();
                // Windows
                services.AddSingleton<LoginWindow>();
                services.AddSingleton<MainWindow>();
                services.AddTransient<PatientDashboardWindow>();
                services.AddTransient<PatientPickerWindow>();
                services.AddTransient<SettingsWindow>();
            })
            .Build();

        _host.Start();

        var settingsService = _host.Services.GetRequiredService<ClientSettingsService>();
        settingsService.InitializeAsync().GetAwaiter().GetResult();

        // Apply language based on settings
        var cultureName = settingsService.Current.UICulture ?? "en";
        var culture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        var localizationService = _host.Services.GetRequiredService<IStringLocalizer>();
        localizationService.CurrentCulture = culture;

        var viewController = _host.Services.GetRequiredService<ViewController>();
        viewController.Load(this);
        // Apply theme before showing any windows
        ThemeManager.ApplyTheme(settingsService.Current.Theme);

        var loginWindow = _host.Services.GetRequiredService<LoginWindow>();

        loginWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}

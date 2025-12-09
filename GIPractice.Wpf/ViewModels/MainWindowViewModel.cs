using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GIPractice.Wpf.Backend;

namespace GIPractice.Wpf.ViewModels;

public sealed class MainWindowViewModel : ScreenViewModelBase
{
    private string _connectionStatusText = "Disconnected";

    public MainWindowViewModel(IDatabase database)
        : base(database)
    {
        Database.ConnectionStateChanged += OnConnectionStateChanged;

        ConnectCommand = new AsyncRelayCommand(ConnectAsync);

        UpdateConnectionStatusText(Database.ConnectionState);
    }

    public ICommand ConnectCommand { get; }

    public string ConnectionStatusText
    {
        get => _connectionStatusText;
        private set => SetProperty(ref _connectionStatusText, value);
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await RunBusyAsync(
            async ct =>
            {
                await Database.EnsureConnectedAsync(ct).ConfigureAwait(false);
                Database.RegisterUserInteraction();
            },
            busyText: "Connecting…",
            externalToken: cancellationToken);
    }

    private void OnConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
        => UpdateConnectionStatusText(e.NewState);

    private void UpdateConnectionStatusText(ConnectionState state)
    {
        ConnectionStatusText = state switch
        {
            ConnectionState.Connected => "Connected",
            ConnectionState.Connecting => "Connecting…",
            ConnectionState.Reconnecting => "Reconnecting…",
            _ => "Disconnected"
        };
    }
}

using System.Threading;
using System.Threading.Tasks;
using GIPractice.Wpf.Backend;

namespace GIPractice.Wpf.ViewModels.Auth;

public sealed class LoginViewModel(IDatabase database) : ScreenViewModelBase(database)
{
    private string _userName = string.Empty;
    private string _serverUrl = database.ServerUrl;

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string ServerUrl
    {
        get => _serverUrl;
        set => SetProperty(ref _serverUrl, value);
    }

    public async Task<bool> LoginAsync(string password, CancellationToken cancellationToken = default)
    {
        var success = false;

        await RunBusyAsync(
            async ct =>
            {
                ErrorMessage = null;
                success = await Database.LoginAsync(UserName, password, ct);
                if (!success)
                {
                    ErrorMessage = "Invalid username or password.";
                }
            },
            busyText: "Logging in…",
            externalToken: cancellationToken);

        return success;
    }
    public void ApplyServerUrl()
    {
        if (!string.Equals(Database.ServerUrl, _serverUrl, StringComparison.OrdinalIgnoreCase))
        {
            Database.SetServerUrl(_serverUrl);
        }
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GIPractice.Client;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly GiPracticeApiClient _api;
    private readonly INavigationService _navigation;

    private string _userName = "admin";
    private string _password = "Admin123!";
    private bool _isBusy;
    private string? _errorMessage;
    private readonly ClientSettingsService _settings;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? RequestClose;

    public LoginViewModel(GiPracticeApiClient api, ClientSettingsService settings, INavigationService navigation)
    {
        _api = api;
        _settings = settings;
        _navigation = navigation;

        if (!string.IsNullOrWhiteSpace(settings.Current.LastUserName))
            _userName = settings.Current.LastUserName;
        else
            _userName = "admin"; // dev default

        LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !IsBusy);
    }

    public string UserName
    {
        get => _userName;
        set { _userName = value; OnPropertyChanged(); }
    }

    // NOTE: for now this is plain string; we can switch to SecureString later.
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            if (LoginCommand is RelayCommand rc)
                rc.RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set { _errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var result = await _api.LoginAsync(UserName, Password);
            if (result == null)
            {
                ErrorMessage = "Login_Error_InvalidCredentials";
                return;
            }

            // persist username (and later language/theme/etc.)
            _settings.Current.LastUserName = UserName;
            await _settings.SaveAsync();

            await _navigation.ShowShellAsync();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

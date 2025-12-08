using System.Threading;
using System.Threading.Tasks;
using GIPractice.ApiClient;
using GIPractice.Client.Core;

namespace GIPractice.Wpf;

public class LoginViewModel : PresentationBase
{
    private readonly ITokenService _tokenService;
    private readonly LocalizationManager _localization;
    private string _userName = string.Empty;
    private string _password = string.Empty;
    private string? _status;
    private bool _isBusy;

    public LoginViewModel(ITokenService tokenService, LocalizationManager localization)
    {
        _tokenService = tokenService;
        _localization = localization;
        SignInCommand = new DelegateCommand(SignInAsync, () => !IsBusy);
    }

    public DelegateCommand SignInCommand { get; }

    public event EventHandler? SignedIn;

    public string UserName
    {
        get => _userName;
        set => SetField(ref _userName, value);
    }

    public string Password
    {
        get => _password;
        set => SetField(ref _password, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            SetField(ref _isBusy, value);
            SignInCommand.RaiseCanExecuteChanged();
        }
    }

    public string? Status
    {
        get => _status;
        private set => SetField(ref _status, value);
    }

    public string Title => _localization["Login.Title"];
    public string UserNameLabel => _localization.Field("Users", "UserName");
    public string PasswordLabel => _localization.Field("Users", "Password");
    public string SubmitLabel => _localization["Login.Submit"];

    private async Task SignInAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        Status = _localization["Status.Connecting"];

        var success = await _tokenService.SignInAsync(UserName, Password, CancellationToken.None).ConfigureAwait(false);
        if (!success)
        {
            Status = _localization["Status.Unauthorized"];
            IsBusy = false;
            return;
        }

        Status = _localization["Status.Connected"];
        IsBusy = false;
        SignedIn?.Invoke(this, EventArgs.Empty);
    }
}

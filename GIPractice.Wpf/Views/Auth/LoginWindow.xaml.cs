using System.Windows;
using GIPractice.Wpf.ViewModels.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace GIPractice.Wpf.Views.Auth;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<LoginViewModel>();
        DataContext = _viewModel;
    }

    private async void OnOkClick(object sender, RoutedEventArgs e)
    {
        var password = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(_viewModel.UserName) ||
            string.IsNullOrWhiteSpace(password))
        {
            _viewModel.ErrorMessage = "Please enter username and password.";
            return;
        }

        // Apply Server URL change (this also persists it)
        _viewModel.ApplyServerUrl();

        var success = await _viewModel.LoginAsync(password);

        if (success)
        {
            DialogResult = true;
            Close();
        }
        else
        {
            PasswordBox.SelectAll();
            PasswordBox.Focus();
        }
    }
}

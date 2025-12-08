using System.Windows;
using GIPractice.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using GIPractice.Client;

namespace GIPractice.Wpf;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;
    private readonly IServiceProvider _services;

    public LoginWindow(LoginViewModel viewModel, IServiceProvider services)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _services = services;

        DataContext = _viewModel;
        _viewModel.LoginSucceeded += ViewModelOnLoginSucceeded;
    }

    private void ViewModelOnLoginSucceeded(object? sender, LoginResponseDto e)
    {
        // Resolve and show the main window
        var main = _services.GetRequiredService<MainWindow>();
        main.Show();

        // Close the login window
        Close();
    }
}

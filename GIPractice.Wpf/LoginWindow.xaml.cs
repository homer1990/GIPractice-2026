using System;
using System.Windows;
using GIPractice.Client;

namespace GIPractice.Wpf;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        DataContext = _viewModel;
        _viewModel.RequestClose += ViewModelOnRequestClose;
    }

    private void ViewModelOnRequestClose(object? sender, EventArgs e)
    {
        Close();
    }
}

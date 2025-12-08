using System;
using System.Threading.Tasks;
using System.Windows;
using GIPractice.Client;

namespace GIPractice.Wpf;

public partial class SettingsWindow : Window
{
    private readonly SettingsViewModel _viewModel;

    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        await _viewModel.InitializeAsync();
    }

    private async void Ok_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.SaveAsync();
        DialogResult = true;
        Close();
    }
}

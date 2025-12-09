using System.Windows;
using GIPractice.Wpf.ViewModels;

namespace GIPractice.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

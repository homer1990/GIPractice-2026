using System.Windows;
using GIPractice.Client;

namespace GIPractice.Wpf;

public partial class MainWindow : Window
{
    private readonly ShellViewModel _shell;

    public MainWindow(ShellViewModel shell)
    {
        InitializeComponent();

        _shell = shell;

        DataContext = _shell;
    }
}

using GIPractice.Wpf.Pages;
using GIPractice.Wpf.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Threading;

namespace GIPractice.Wpf;

public partial class MainWindow : MetroWindow
{
    private readonly Home home = new();

    public MainWindow()
    {
        InitializeComponent();

        this.Root.Content = this.home;
        DataContext = new ShellViewModel();
    }
}

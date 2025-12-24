using GIPractice.Wpf.Pages;
using GIPractice.Wpf.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Threading;

namespace GIPractice.Wpf;

public partial class MainWindow : MetroWindow
{
    private Home home;
    private IDialogCoordinator _dc;
    public IDialogCoordinator GetCoordinator()
    {
        return _dc;
    }
    private static Size GetDesiredSize(FrameworkElement element)
    {
        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return element.DesiredSize;
    }

    private void UpdateMinSizeToFit(FrameworkElement root)
    {
        var desired = GetDesiredSize(root);

        // must run after layout at least once
        var chromeW = ActualWidth - root.ActualWidth;
        var chromeH = ActualHeight - root.ActualHeight;

        MinWidth = desired.Width + chromeW;
        MinHeight = desired.Height + chromeH;
    }

    public MainWindow()
    {
        InitializeComponent();

        this.home = new Home();
        this.Root.Content = this.home;
        _dc = (DialogCoordinator.Instance);
        DataContext = new ShellViewModel();
        /*
        Loaded += (_, __) =>
        {
            // run after first layout pass
            Dispatcher.BeginInvoke(
                DispatcherPriority.Loaded,
                new Action(() => UpdateMinSizeToFit(Root)));
        };
        */
    }
}

using MahApps.Metro.Controls.Dialogs;

namespace GIPractice.Wpf.ViewModels;

public sealed class ShellViewModel
{
    public ShellViewModel()
    {
        Flyout = new FlyoutHostViewModel();
    }
    public FlyoutHostViewModel Flyout { get; }
}

using GIPractice.Client.Core;

namespace GIPractice.Wpf;

public class ShellViewModel
{
    public ShellViewModel(ViewController viewController, LoginViewModel login, DashboardViewModel dashboard, IDatabaseController database)
    {
        ViewController = viewController;
        Login = login;
        Dashboard = dashboard;

        ViewController.Navigate(Login);
        Login.SignedIn += (_, _) => ViewController.Navigate(Dashboard);

        database.InterruptRaised += (_, signal) =>
        {
            if (signal.Reason is InterruptReason.Unauthorized or InterruptReason.Inactivity)
            {
                ViewController.Navigate(Login);
            }
        };
    }

    public ViewController ViewController { get; }
    public LoginViewModel Login { get; }
    public DashboardViewModel Dashboard { get; }
}

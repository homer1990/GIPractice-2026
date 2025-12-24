using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace GIPractice.Wpf.Pages
{
    public partial class Home : Page
    {
        private const string ExitTag = "__EXIT__";

        // Map your Tags -> page URIs
        private static readonly Dictionary<string, Uri> Routes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Patients"] = new Uri("/GIPractice.Wpf;component/Pages/Patients.xaml", UriKind.Relative),
            ["Calendar"] = new Uri("/GIPractice.Wpf;component/Pages/Calendar.xaml", UriKind.Relative),
            ["Endoscopies"] = new Uri("/GIPractice.Wpf;component/Pages/Endoscopies.xaml", UriKind.Relative),

            ["Settings"] = new Uri("/GIPractice.Wpf;component/Pages/Settings.xaml", UriKind.Relative),
            ["Help"] = new Uri("/GIPractice.Wpf;component/Pages/Help.xaml", UriKind.Relative),
        };

        public Home()
        {
            InitializeComponent();

            Loaded += Home_Loaded;
            MainFrame.NavigationFailed += MainFrame_NavigationFailed;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            // Default page when Home loads
            NavigateByTag("Patients");
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is not HamburgerMenuItem item)
                return;

            if (item.Tag is not string tag)
                return;

            if (string.Equals(tag, ExitTag, StringComparison.Ordinal))
            {
                Application.Current.Shutdown();
                return;
            }

            NavigateByTag(tag);
        }

        private void NavigateByTag(string tag)
        {
            if (!Routes.TryGetValue(tag, out var uri))
            {
                MessageBox.Show($"No route configured for '{tag}'.", "Navigation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Avoid re-navigating to the same page
            if (MainFrame.Source != null &&
                Uri.Compare(MainFrame.Source, uri, UriComponents.Path, UriFormat.SafeUnescaped,
                    StringComparison.OrdinalIgnoreCase) == 0)
            {
                return;
            }

            MainFrame.Navigate(uri);
        }

        private void MainFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            MessageBox.Show(
                $"Failed to load page:\n{e.Uri}\n\n{e.Exception.Message}",
                "Navigation",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

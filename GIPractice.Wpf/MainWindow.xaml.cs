using GIPractice.Wpf.ViewModels;
using GIPractice.Wpf.ViewModels.Patients;
using GIPractice.Wpf.Views.Auth;
using System.Windows;

namespace GIPractice.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(
        MainWindowViewModel shellViewModel,
        PatientSearchViewModel patientSearchViewModel)
    {
        InitializeComponent();

        DataContext = shellViewModel;
        PatientSearchViewControl.DataContext = patientSearchViewModel;
    }
    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        var win = new LoginWindow
        {
            Owner = this
        };

        win.ShowDialog();
        // ConnectionStatusText will update via Database.ConnectionStateChanged
    }
}

using GIPractice.Wpf.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace GIPractice.Wpf.Pages;

public partial class Patients : Page
{
    private static ShellViewModel? Shell =>
        Application.Current.MainWindow?.DataContext as ShellViewModel;

    private void OpenFlyout_Click(object sender, RoutedEventArgs e)
    {
        if (Shell is null) return;
        if (sender is not FrameworkElement fe) return;
        if (fe.Tag is not string key) return;

        object content;
        string header;
        double width;
        var position = Position.Right;

        switch (key)
        {
            case "Search":
                header = "ΑΝΑΖΗΤΗΣΗ";
                content = new PatientSearchVm();
                width = 1200;
                position = Position.Left;
                break;

            case "NewVisit":
                header = "ΝΕΑ ΕΠΙΣΚΕΨΗ";
                content = new NewVisitVm();
                width = 520;
                break;

            case "NewAppointment":
                header = "ΝΕΟ ΡΑΝΤΕΒΟΥ";
                content = new NewAppointmentVm();
                width = 520;
                break;

            case "NewEndoscopy":
                header = "ΝΕΑ ΕΝΔΟΣΚΟΠΗΣΗ";
                content = new NewEndoscopyVm();
                width = 640;
                break;

            case "NewPathologyReport":
                header = "ΝΕΑ ΠΑΘΟΛΟΓΟΑΝΑΤΟΜΙΚΗ";
                content = new NewPathologyReportVm();
                width = 640;
                break;

            default:
                return;
        }

        Shell.Flyout.Show(content, header: header, position: position, width: width, modal: false);
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        return;
    }
    private void CapturePhoto_Click(object sender, RoutedEventArgs e)
    {
        return;
    }
    private void PhotoFromFile_Click(object sender, RoutedEventArgs e)
    {
        return;
    }
    private async void CRUD_Click(object sender, RoutedEventArgs e)
    {
        var mw = (MetroWindow)Application.Current.MainWindow!;
        var context = mw.DataContext;
        var gett = await mw.ShowInputAsync("TESTING", "INPUT TEST VALUE");
        await mw.ShowMessageAsync("TEST", "THIS IS THE TEST VALUE: " + gett);
    }
}
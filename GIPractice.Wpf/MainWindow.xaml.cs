using System.Windows;
using GIPractice.Wpf.ViewModels;
using GIPractice.Wpf.ViewModels.Patients;
using GIPractice.Wpf.Views.Patients;

namespace GIPractice.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(
        MainWindowViewModel shellViewModel,
        PatientSearchViewModel patientSearchViewModel,
        PatientsSearchView patientsSearchView)
    {
        InitializeComponent();

        // Shell VM for window-level stuff (status bar, connect button).
        DataContext = shellViewModel;

        // Assign the PatientsSearchView DataContext explicitly:
        patientsSearchView.DataContext = patientSearchViewModel;
    }
}

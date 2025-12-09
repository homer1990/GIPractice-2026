using System.Windows;
using GIPractice.Wpf.ViewModels;
using GIPractice.Wpf.ViewModels.Patients;

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
}

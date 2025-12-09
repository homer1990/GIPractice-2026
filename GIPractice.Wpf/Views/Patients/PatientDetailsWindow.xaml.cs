using System.Windows;

namespace GIPractice.Wpf.Views.Patients;

public partial class PatientDetailsWindow : Window
{
    public PatientDetailsWindow()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

using System.Threading.Tasks;
using System.Windows;
using GIPractice.Api.Models;
using GIPractice.Client;

namespace GIPractice.Wpf;

public partial class PatientPickerWindow : Window
{
    private readonly PatientSearchViewModel _viewModel;

    public PatientPickerWindow(PatientSearchViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        _viewModel.SelectionHandler = patient =>
        {
            OnPatientChosen(patient);
            return Task.CompletedTask;
        };
    }

    public PatientListItemDto? SelectedPatient { get; private set; }

    private void OnPatientChosen(PatientListItemDto patient)
    {
        SelectedPatient = patient;
        DialogResult = true;
        Close();
    }
}

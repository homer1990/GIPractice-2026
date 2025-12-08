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

        _viewModel.OpenPatientDetailsRequested += OnPatientChosen;
    }

    public PatientListItemDto? SelectedPatient { get; private set; }

    private void OnPatientChosen(object? sender, PatientListItemDto patient)
    {
        SelectedPatient = patient;
        DialogResult = true;
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _viewModel.OpenPatientDetailsRequested -= OnPatientChosen;
    }
}

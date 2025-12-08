using System.Threading.Tasks;
using System.Windows;
using GIPractice.Api.Models;
using GIPractice.Client;

namespace GIPractice.Wpf;

public partial class PatientDashboardWindow : Window
{
    private readonly PatientDashboardViewModel _viewModel;

    public PatientDashboardWindow(PatientDashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    public async Task InitializeAsync(PatientListItemDto patient)
    {
        await _viewModel.LoadAsync(patient);
    }
}

using System.Windows.Controls;
using GIPractice.Client;
using GIPractice.Wpf.Views.Common;

namespace GIPractice.Wpf.Views.Patients;

public partial class PatientSearchView : SearchViewBase<PatientSearchViewModel>
{
    public PatientSearchView()
    {
        InitializeComponent();
    }

    private async void PatientsGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        await HandleSortingAsync(sender, e);
    }
}

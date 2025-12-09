using GIPractice.Wpf.Backend;
using GIPractice.Wpf.ViewModels;
using GIPractice.Wpf.ViewModels.Patients;
using GIPractice.Wpf.Views.Patients;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;

namespace GIPractice.Wpf.Views.Patients;

public partial class PatientsSearchView : UserControl
{
    public PatientsSearchView()
    {
        InitializeComponent();
    }

    private async void OnDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not PatientSearchViewModel vm)
            return;

        var selected = vm.SelectedItem;
        if (selected is null)
            return;

        // ASSUMPTION: PatientSummaryDto has an int Id
        var db = App.Services.GetRequiredService<IDatabase>();
        var detailsVm = new PatientDetailsViewModel(db, selected.PatientId);

        var window = new PatientDetailsWindow
        {
            Owner = System.Windows.Window.GetWindow(this),
            DataContext = detailsVm
        };

        // Close window when save completes successfully
        detailsVm.Saved += (_, _) =>
        {
            window.DialogResult = true;
            window.Close();
            detailsVm.Saved += (_, _) =>
            {
                window.DialogResult = true;
                window.Close();

                // optional refresh:
                if (DataContext is PatientSearchViewModel searchVm &&
                    searchVm.SearchCommand is AsyncRelayCommand cmd)
                {
                    _ = cmd.ExecuteAsync();
                }
            };
        };

        window.Show();

        await detailsVm.LoadAsync();
    }
    private async void OnNewPatientClick(object sender, System.Windows.RoutedEventArgs e)
    {
        var db = App.Services.GetRequiredService<IDatabase>();
        var detailsVm = new PatientDetailsViewModel(db, id: 0); // 0 => new

        var window = new PatientDetailsWindow
        {
            Owner = System.Windows.Window.GetWindow(this),
            DataContext = detailsVm
        };

        detailsVm.Saved += (_, _) =>
        {
            window.DialogResult = true;
            window.Close();

            // Optional: refresh search after creating a new patient
            if (DataContext is PatientSearchViewModel searchVm &&
                searchVm.SearchCommand is AsyncRelayCommand cmd)
            {
                _ = cmd.ExecuteAsync(); // fire and forget
            }
        };

        window.Show();

        // For a new item we may not need LoadAsync; but calling it is harmless
        await detailsVm.LoadAsync();
    }

}

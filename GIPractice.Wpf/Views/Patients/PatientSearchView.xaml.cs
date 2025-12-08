using System.ComponentModel;
using System.Windows.Controls;
using GIPractice.Client; // where PatientSearchViewModel lives

namespace GIPractice.Wpf.Views.Patients;

public partial class PatientSearchView : UserControl
{
    public PatientSearchView()
    {
        InitializeComponent();
    }

    private async void PatientsGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        // Cancel WPF's local sorting
        e.Handled = true;

        if (DataContext is not PatientSearchViewModel vm)
            return;

        var column = e.Column;

        // Determine new sort direction
        var newDirection = column.SortDirection != ListSortDirection.Ascending
            ? ListSortDirection.Ascending
            : ListSortDirection.Descending;

        // Reset other columns' sort arrows
        if (sender is DataGrid grid)
        {
            foreach (var col in grid.Columns)
            {
                if (!ReferenceEquals(col, column))
                    col.SortDirection = null;
            }
        }

        column.SortDirection = newDirection;

        // Map column to server sort field
        var sortField = column.SortMemberPath ?? column.Header?.ToString() ?? "LastVisit";

        vm.SortField = sortField;
        vm.SortDescending = newDirection == ListSortDirection.Descending;

        // Every new sort starts from page 1
        vm.Page = 1;

        await vm.SearchAsync();
    }
}

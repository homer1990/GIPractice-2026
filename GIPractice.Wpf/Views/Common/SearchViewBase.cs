using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using GIPractice.Client;

namespace GIPractice.Wpf.Views.Common;

public abstract class SearchViewBase<TViewModel> : UserControl
    where TViewModel : class, ISearchViewModel, INotifyPropertyChanged
{
    protected async Task HandleSortingAsync(object sender, DataGridSortingEventArgs e)
    {
        e.Handled = true;

        if (DataContext is not TViewModel vm)
            return;

        var column = e.Column;
        var newDirection = column.SortDirection != ListSortDirection.Ascending
            ? ListSortDirection.Ascending
            : ListSortDirection.Descending;

        if (sender is DataGrid grid)
        {
            foreach (var col in grid.Columns)
            {
                if (!ReferenceEquals(col, column))
                    col.SortDirection = null;
            }
        }

        column.SortDirection = newDirection;

        var sortField = column.SortMemberPath ?? column.Header?.ToString() ?? string.Empty;
        await vm.ApplySortAsync(sortField, newDirection == ListSortDirection.Descending);
    }
}

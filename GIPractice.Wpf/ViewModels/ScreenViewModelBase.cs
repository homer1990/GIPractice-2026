using System;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Wpf.Backend;

namespace GIPractice.Wpf.ViewModels;

public abstract class ScreenViewModelBase(IDatabase database) : ViewModelBase
{
    private bool _isBusy;
    private string? _busyText;
    private string? _errorMessage;

    protected IDatabase Database { get; } = database ?? throw new ArgumentNullException(nameof(database));

    public bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    public string? BusyText
    {
        get => _busyText;
        protected set => SetProperty(ref _busyText, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        protected set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// Helper to run an async operation with IsBusy/ErrorMessage handling.
    /// </summary>
    protected async Task RunBusyAsync(
        Func<CancellationToken, Task> operation,
        string? busyText = null,
        CancellationToken externalToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        IsBusy = true;
        BusyText = busyText;
        ErrorMessage = null;

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            await operation(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // ignore, or set a message if you like
        }
        catch (Exception ex)
        {
            // later: centralise logging
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            BusyText = null;
        }
    }
}

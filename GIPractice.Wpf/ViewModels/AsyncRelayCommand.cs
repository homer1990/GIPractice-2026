using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GIPractice.Wpf.ViewModels;

public sealed class AsyncRelayCommand(
    Func<CancellationToken, Task> execute,
    Func<bool>? canExecute = null) : ICommand
{
    private readonly Func<CancellationToken, Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Func<bool>? _canExecute = canExecute;
    private bool _isExecuting;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
        => !_isExecuting && (_canExecute?.Invoke() ?? true);

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            using var cts = new CancellationTokenSource();
            await _execute(cts.Token).ConfigureAwait(false);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    // in AsyncRelayCommand.cs
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!CanExecute(null))
            return Task.CompletedTask;

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return _execute(cts.Token);
    }

}

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GIPractice.Client;

public class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
{
    private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Func<object?, bool>? _canExecute = canExecute;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null) : ICommand
{
    private readonly Func<object?, Task> _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
    private readonly Func<object?, bool>? _canExecute = canExecute;
    private bool _isExecuting;

    public bool CanExecute(object? parameter) =>
        !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

    // Non-blocking ICommand Execute that starts the async operation.
    public void Execute(object? parameter) => _ = ExecuteAsync(parameter);

    // Public API to await the operation if desired.
    public Task ExecuteAsync(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return Task.CompletedTask;
        }

        return ExecuteInternalAsync(parameter);
    }

    private async Task ExecuteInternalAsync(object? parameter)
    {
        _isExecuting = true;
        RaiseCanExecuteChanged();
        try
        {
            await _executeAsync(parameter).ConfigureAwait(false);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GIPractice.Client.Core;

namespace GIPractice.Wpf;

public class ViewController : INotifyPropertyChanged
{
    private readonly IDatabaseController _database;
    private object? _currentViewModel;
    private string _statusMessage = string.Empty;
    private bool _isDimmed;
    private string? _overlay;

    public ViewController(IDatabaseController database)
    {
        _database = database;
        _database.StatusChanged += OnStatusChanged;
        _database.InterruptRaised += OnInterruptRaised;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetField(ref _currentViewModel, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public bool IsDimmed
    {
        get => _isDimmed;
        private set => SetField(ref _isDimmed, value);
    }

    public string? Overlay
    {
        get => _overlay;
        private set => SetField(ref _overlay, value);
    }

    public void Navigate(object viewModel)
    {
        CurrentViewModel = viewModel;
        Overlay = null;
        IsDimmed = false;
    }

    private void OnStatusChanged(object? sender, ConnectionStateChange e)
    {
        StatusMessage = e.Message ?? e.Status.ToString();
        IsDimmed = e.Status == ConnectionStatus.Disconnected;
    }

    private void OnInterruptRaised(object? sender, InterruptSignal e)
    {
        Overlay = e.Message;
        IsDimmed = true;
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

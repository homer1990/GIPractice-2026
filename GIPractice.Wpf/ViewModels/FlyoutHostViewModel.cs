using System.ComponentModel;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GIPractice.Wpf.ViewModels;

public sealed class FlyoutHostViewModel : INotifyPropertyChanged
{
    private bool _isOpen;
    private bool _isModal;
    private double _width = 520;
    private Position _position = Position.Left;
    private object? _content;
    private string? _header;

    public bool IsOpen { get => _isOpen; set => Set(ref _isOpen, value); }
    public bool IsModal { get => _isModal; set => Set(ref _isModal, value); }
    public double Width { get => _width; set => Set(ref _width, value); }
    public Position Position { get => _position; set => Set(ref _position, value); }
    public object? Content { get => _content; set => Set(ref _content, value); }
    public string? Header { get => _header; set => Set(ref _header, value); }

    public void Show(object content, string? header = null, Position position = Position.Left, double width = 520, bool modal = false)
    {
        Header = header;
        Position = position;
        Width = width;
        IsModal = modal;
        Content = content;
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
        // optional:
        // Content = null;
        // Header = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

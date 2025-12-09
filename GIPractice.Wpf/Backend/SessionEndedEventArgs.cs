using System;

namespace GIPractice.Wpf.Backend;

public sealed class SessionEndedEventArgs(SessionEndedReason reason, string? message = null) : EventArgs
{
    public SessionEndedReason Reason { get; } = reason;

    public string? Message { get; } = message;
}

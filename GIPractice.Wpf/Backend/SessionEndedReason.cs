namespace GIPractice.Wpf.Backend;

public enum SessionEndedReason
{
    Unknown,
    Inactivity,
    ExplicitLogout,
    TokenExpired,
    ConnectionLost
}

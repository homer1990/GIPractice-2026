namespace GIPractice.Contracts.Auth;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Reception = "Reception";
    public const string ReadOnly = "ReadOnly";

    public static readonly string[] All =
    [
        Admin,
        Doctor,
        Reception,
        ReadOnly
    ];
}
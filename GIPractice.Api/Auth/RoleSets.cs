using GIPractice.Contracts.Auth;

namespace GIPractice.Api.Auth;

public static class RoleSets
{
    public const string AdminOnly = AppRoles.Admin;
    public const string Clinician = AppRoles.Admin + "," + AppRoles.Doctor;
    public const string Staff = AppRoles.Admin + "," + AppRoles.Doctor + "," + AppRoles.Reception;
    public const string ReadOnlyOrBetter = AppRoles.Admin + "," + AppRoles.Doctor + "," + AppRoles.Reception + "," + AppRoles.ReadOnly;
}

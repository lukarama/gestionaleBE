namespace Gestionale.Api.Common.Auth;

public static class RequestRoleExtensions
{
    public static bool IsAdminOrResponsabile(this UserContext user)
    {
        return user.Roles.Any(role =>
            string.Equals(role, RoleCodes.Admin, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, RoleCodes.Responsabile, StringComparison.OrdinalIgnoreCase));
    }
}

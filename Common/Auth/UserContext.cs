namespace Gestionale.Api.Common.Auth;

public class UserContext
{
    public int? UserId { get; set; }
    public int? DipendenteId { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();

    public bool IsAdmin => Roles.Any(role => string.Equals(role, RoleCodes.Admin, StringComparison.OrdinalIgnoreCase));
    public bool IsDipendente => Roles.Any(role => string.Equals(role, RoleCodes.Dipendente, StringComparison.OrdinalIgnoreCase));
}

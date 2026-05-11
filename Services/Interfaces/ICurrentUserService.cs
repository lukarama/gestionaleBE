using System.Security.Claims;

namespace Gestionale.Api.Services.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    int? UserId { get; }

    int? DipendenteId { get; }

    ClaimsPrincipal Principal { get; }

    bool HasPermission(string permissionCode);

    bool HasAnyRole(params string[] roles);
}

using System.Security.Claims;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Gestionale.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated == true;

    public int? UserId => TryGetIntClaim(ClaimTypes.NameIdentifier);

    public int? DipendenteId => TryGetIntClaim(CustomClaimTypes.DipendenteId);

    public ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

    public bool HasPermission(string permissionCode)
    {
        return Principal.Claims.Any(claim =>
            claim.Type == CustomClaimTypes.Permission &&
            string.Equals(claim.Value, permissionCode, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasAnyRole(params string[] roles)
    {
        return roles.Any(role => Principal.IsInRole(role));
    }

    private int? TryGetIntClaim(string claimType)
    {
        var value = Principal.FindFirstValue(claimType);
        return int.TryParse(value, out var parsed) ? parsed : null;
    }
}

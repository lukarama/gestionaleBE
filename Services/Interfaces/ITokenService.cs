using Gestionale.Api.Models;

namespace Gestionale.Api.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(Utenti user, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions);

    string GenerateRefreshToken();

    string ComputeRefreshTokenHash(string refreshToken);
}

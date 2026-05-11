using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Options;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gestionale.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(AuthLoginRequestDto request, string? ipAddress, string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<AuthResponseDto>.Fail("Username/email e password sono obbligatori.", 400);
        }

        var normalizedInput = request.UsernameOrEmail.Trim();
        var user = await _dbContext.Utentis
            .Include(u => u.UtentiRuolis)
                .ThenInclude(ur => ur.Ruolo)
                    .ThenInclude(r => r.RuoliPermessis)
                        .ThenInclude(rp => rp.Permesso)
            .FirstOrDefaultAsync(u =>
                u.Username == normalizedInput ||
                u.Email == normalizedInput);

        if (user == null || !user.Attivo || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ServiceResult<AuthResponseDto>.Fail("Credenziali non valide.", 401);
        }

        user.UltimoAccessoAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        var response = await BuildAuthResponseAsync(user, ipAddress, userAgent);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<AuthResponseDto>.Ok(response);
    }

    public async Task<ServiceResult<AuthResponseDto>> RefreshAsync(AuthRefreshRequestDto request, string? ipAddress, string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return ServiceResult<AuthResponseDto>.Fail("Refresh token obbligatorio.", 400);
        }

        var refreshTokenHash = _tokenService.ComputeRefreshTokenHash(request.RefreshToken);

        var tokenEntity = await _dbContext.RefreshTokens
            .Include(rt => rt.Utente)
                .ThenInclude(u => u.UtentiRuolis)
                    .ThenInclude(ur => ur.Ruolo)
                        .ThenInclude(r => r.RuoliPermessis)
                            .ThenInclude(rp => rp.Permesso)
            .FirstOrDefaultAsync(rt => rt.TokenHash == refreshTokenHash);

        if (tokenEntity == null || !tokenEntity.IsActive || !tokenEntity.Utente.Attivo)
        {
            return ServiceResult<AuthResponseDto>.Fail("Refresh token non valido.", 401);
        }

        tokenEntity.RevokedAt = DateTime.UtcNow;
        var response = await BuildAuthResponseAsync(tokenEntity.Utente, ipAddress, userAgent);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<AuthResponseDto>.Ok(response);
    }

    public async Task<ServiceResult<bool>> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return ServiceResult<bool>.Ok(true);
        }

        var refreshTokenHash = _tokenService.ComputeRefreshTokenHash(refreshToken);
        var tokenEntity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == refreshTokenHash);

        if (tokenEntity == null)
        {
            return ServiceResult<bool>.Ok(true);
        }

        tokenEntity.RevokedAt ??= DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<AuthUserProfileDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _dbContext.Utentis
            .Include(u => u.UtentiRuolis)
                .ThenInclude(ur => ur.Ruolo)
                    .ThenInclude(r => r.RuoliPermessis)
                        .ThenInclude(rp => rp.Permesso)
            .FirstOrDefaultAsync(u => u.Id == userId && u.Attivo);

        if (user == null)
        {
            return null;
        }

        var permissions = ExtractPermissions(user);
        var roles = ExtractRoles(user);

        return new AuthUserProfileDto
        {
            UserId = user.Id,
            Username = user.Username ?? string.Empty,
            Email = user.Email ?? string.Empty,
            NomeCompleto = $"{user.Nome ?? string.Empty} {user.Cognome ?? string.Empty}".Trim(),
            DipendenteId = user.DipendenteId,
            Roles = roles,
            Permissions = permissions
        };
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(Utenti user, string? ipAddress, string? userAgent)
    {
        var roles = ExtractRoles(user);

        var permissions = ExtractPermissions(user);
        var (accessToken, expiresAtUtc) = _tokenService.CreateAccessToken(user, roles, permissions);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _dbContext.RefreshTokens.Add(new RefreshTokens
        {
            UtenteId = user.Id,
            TokenHash = _tokenService.ComputeRefreshTokenHash(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
            CreatedByIp = ipAddress,
            UserAgent = userAgent
        });

        await _dbContext.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAtUtc = expiresAtUtc,
            User = new CurrentUserDto
            {
                UserId = user.Id,
                Username = user.Username ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Nome = user.Nome ?? string.Empty,
                Cognome = user.Cognome ?? string.Empty,
                DipendenteId = user.DipendenteId,
                MustChangePassword = user.MustChangePassword,
                Roles = roles,
                Permissions = permissions
            }
        };
    }

    private static string[] ExtractPermissions(Utenti user)
    {
        return user.UtentiRuolis
            .Where(ur => ur.Ruolo.Attivo)
            .SelectMany(ur => ur.Ruolo.RuoliPermessis)
            .Where(rp => rp.Permesso.Attivo)
            .Select(rp => rp.Permesso.Codice)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string[] ExtractRoles(Utenti user)
    {
        return user.UtentiRuolis
            .Where(ur => ur.Ruolo.Attivo)
            .Select(ur => ur.Ruolo.Nome.Trim().ToUpperInvariant())
            .Where(role => role.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}

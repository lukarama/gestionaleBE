using System.Security.Cryptography;
using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class UtentiAdminService : IUtentiAdminService
{
    private static readonly string[] AllowedRoles =
    [
        RoleCodes.Admin,
        RoleCodes.Responsabile,
        RoleCodes.Magazziniere,
        RoleCodes.Dipendente,
        RoleCodes.Segreteria
    ];

    private static readonly string[] AllowedVisibility =
    [
        "dashboard",
        "dipendenti",
        "magazzino",
        "attrezzature",
        "dpi",
        "mezzi",
        "cantieri",
        "segreteria",
        "ferie-permessi",
        "richieste-aziendali"
    ];

    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UtentiAdminService(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UtenteDipendenteAdminDto>> GetDipendentiAccountsAsync()
    {
        var dipendenti = await _context.Dipendentis
            .AsNoTracking()
            .Include(d => d.Utentis)
                .ThenInclude(u => u.UtentiRuolis)
                    .ThenInclude(ur => ur.Ruolo)
            .Include(d => d.Utentis)
                .ThenInclude(u => u.UtentiVisibilitas)
            .OrderBy(d => d.Cognome)
            .ThenBy(d => d.Nome)
            .ToListAsync();

        return dipendenti.Select(MapDipendente).ToList();
    }

    public async Task<ServiceResult<UtenteDipendenteAdminDto>> CreateDipendenteAccountAsync(CreateUtenteDipendenteDto dto)
    {
        var username = Clean(dto.Username);
        var email = Clean(dto.Email);

        if (string.IsNullOrWhiteSpace(username))
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Username obbligatorio.");
        }

        if (string.IsNullOrWhiteSpace(dto.TemporaryPassword))
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Password temporanea obbligatoria.");
        }

        var normalizedRolesResult = NormalizeRoles(dto.Roles);
        if (!normalizedRolesResult.Success)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail(normalizedRolesResult.Message!);
        }

        var normalizedVisibilityResult = NormalizeVisibility(dto.Visibility);
        if (!normalizedVisibilityResult.Success)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail(normalizedVisibilityResult.Message!);
        }

        var dipendente = await _context.Dipendentis
            .FirstOrDefaultAsync(d => d.Id == dto.DipendenteId);

        if (dipendente == null)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Dipendente non trovato.", 404);
        }

        if (await _context.Utentis.AnyAsync(u => u.DipendenteId == dto.DipendenteId))
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Il dipendente ha già un account.");
        }

        if (await UsernameExistsAsync(username))
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Username già utilizzato.");
        }

        if (email != null && await EmailExistsAsync(email))
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Email già utilizzata.");
        }

        var roles = await GetValidRolesAsync(normalizedRolesResult.Data!);
        if (roles == null)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Ruolo non valido.");
        }

        var now = DateTime.UtcNow;
        var user = new Utenti
        {
            DipendenteId = dipendente.Id,
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(dto.TemporaryPassword),
            Nome = dipendente.Nome,
            Cognome = dipendente.Cognome,
            Attivo = true,
            MustChangePassword = true,
            CreatedAt = now
        };

        foreach (var role in roles)
        {
            user.UtentiRuolis.Add(new UtentiRuoli
            {
                RuoloId = role.Id,
                CreatedAt = now
            });
        }

        foreach (var key in normalizedVisibilityResult.Data!)
        {
            user.UtentiVisibilitas.Add(new UtentiVisibilita
            {
                Chiave = key,
                CreatedAt = now
            });
        }

        _context.Utentis.Add(user);
        await _context.SaveChangesAsync();

        var result = await GetByUserIdAsync(user.Id);
        return ServiceResult<UtenteDipendenteAdminDto>.Created(result!);
    }

    public async Task<ServiceResult<UtenteDipendenteAdminDto>> UpdateVisibilityAsync(int userId, UpdateUtenteVisibilityDto dto)
    {
        var normalizedRolesResult = NormalizeRoles(dto.Roles);
        if (!normalizedRolesResult.Success)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail(normalizedRolesResult.Message!);
        }

        var normalizedVisibilityResult = NormalizeVisibility(dto.Visibility);
        if (!normalizedVisibilityResult.Success)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail(normalizedVisibilityResult.Message!);
        }

        var user = await _context.Utentis
            .Include(u => u.UtentiRuolis)
                .ThenInclude(ur => ur.Ruolo)
            .Include(u => u.UtentiVisibilitas)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Utente non trovato.", 404);
        }

        var roles = await GetValidRolesAsync(normalizedRolesResult.Data!);
        if (roles == null)
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Ruolo non valido.");
        }

        var currentlyAdmin = UserHasAdminRole(user);
        var willBeAdmin = roles.Any(r => string.Equals(r.Nome, RoleCodes.Admin, StringComparison.OrdinalIgnoreCase));
        if (currentlyAdmin && !willBeAdmin && await IsLastActiveAdminAsync(user.Id))
        {
            return ServiceResult<UtenteDipendenteAdminDto>.Fail("Non è possibile rimuovere o disattivare l’ultimo amministratore attivo.");
        }

        var now = DateTime.UtcNow;
        _context.UtentiRuolis.RemoveRange(user.UtentiRuolis);
        _context.UtentiVisibilitas.RemoveRange(user.UtentiVisibilitas);

        foreach (var role in roles)
        {
            _context.UtentiRuolis.Add(new UtentiRuoli
            {
                UtenteId = user.Id,
                RuoloId = role.Id,
                CreatedAt = now
            });
        }

        foreach (var key in normalizedVisibilityResult.Data!)
        {
            _context.UtentiVisibilitas.Add(new UtentiVisibilita
            {
                UtenteId = user.Id,
                Chiave = key,
                CreatedAt = now
            });
        }

        user.UpdatedAt = now;
        await _context.SaveChangesAsync();

        var result = await GetByUserIdAsync(user.Id);
        return result == null
            ? ServiceResult<UtenteDipendenteAdminDto>.Fail("Utente non trovato.", 404)
            : ServiceResult<UtenteDipendenteAdminDto>.Ok(result);
    }

    public async Task<ServiceResult<ResetUtentePasswordResultDto>> ResetPasswordAsync(int userId, ResetUtentePasswordDto? dto = null)
    {
        var user = await _context.Utentis.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return ServiceResult<ResetUtentePasswordResultDto>.Fail("Utente non trovato.", 404);
        }

        var temporaryPassword = Clean(dto?.TemporaryPassword) ?? GenerateTemporaryPassword();
        user.PasswordHash = _passwordHasher.HashPassword(temporaryPassword);
        user.MustChangePassword = dto?.MustChangePassword ?? true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ServiceResult<ResetUtentePasswordResultDto>.Ok(new ResetUtentePasswordResultDto
        {
            TemporaryPassword = temporaryPassword,
            MustChangePassword = user.MustChangePassword
        });
    }

    public async Task<ServiceResult<bool>> DeleteAccountAsync(int userId)
    {
        var user = await _context.Utentis
            .Include(u => u.UtentiRuolis)
                .ThenInclude(ur => ur.Ruolo)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return ServiceResult<bool>.Fail("Utente non trovato.", 404);
        }

        if (user.Attivo && UserHasAdminRole(user) && await IsLastActiveAdminAsync(user.Id))
        {
            return ServiceResult<bool>.Fail("Non è possibile rimuovere o disattivare l’ultimo amministratore attivo.");
        }

        var now = DateTime.UtcNow;
        user.Attivo = false;
        user.UpdatedAt = now;

        var activeRefreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UtenteId == user.Id && rt.RevokedAt == null && rt.ExpiresAt > now)
            .ToListAsync();

        foreach (var token in activeRefreshTokens)
        {
            token.RevokedAt = now;
        }

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private async Task<UtenteDipendenteAdminDto?> GetByUserIdAsync(int userId)
    {
        var user = await _context.Utentis
            .AsNoTracking()
            .Include(u => u.Dipendente)
            .Include(u => u.UtentiRuolis)
                .ThenInclude(ur => ur.Ruolo)
            .Include(u => u.UtentiVisibilitas)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Dipendente == null ? null : MapUser(user);
    }

    private async Task<List<Ruoli>?> GetValidRolesAsync(IReadOnlyCollection<string> roleNames)
    {
        var roles = await _context.Ruolis
            .Where(r => r.Attivo && roleNames.Contains(r.Nome.ToUpper()))
            .ToListAsync();

        return roles.Count == roleNames.Count ? roles : null;
    }

    private async Task<bool> UsernameExistsAsync(string username)
    {
        var normalized = username.ToUpper();
        return await _context.Utentis.AnyAsync(u => u.Username.ToUpper() == normalized);
    }

    private async Task<bool> EmailExistsAsync(string email)
    {
        var normalized = email.ToUpper();
        return await _context.Utentis.AnyAsync(u => u.Email != null && u.Email.ToUpper() == normalized);
    }

    private async Task<bool> IsLastActiveAdminAsync(int userId)
    {
        var activeAdminIds = await _context.UtentiRuolis
            .Where(ur => ur.Utente.Attivo &&
                         ur.Ruolo.Attivo &&
                         ur.Ruolo.Nome.ToUpper() == RoleCodes.Admin)
            .Select(ur => ur.UtenteId)
            .Distinct()
            .ToListAsync();

        return activeAdminIds.Count == 1 && activeAdminIds[0] == userId;
    }

    private static ServiceResult<string[]> NormalizeRoles(IEnumerable<string>? roles)
    {
        var normalized = (roles ?? [])
            .Select(Clean)
            .Where(role => role != null)
            .Select(role => role!.ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return normalized.Length == 0
            ? ServiceResult<string[]>.Fail("Seleziona almeno un ruolo.")
            : normalized.Any(role => !AllowedRoles.Contains(role))
                ? ServiceResult<string[]>.Fail("Ruolo non valido.")
                : ServiceResult<string[]>.Ok(normalized);
    }

    private static ServiceResult<string[]> NormalizeVisibility(IEnumerable<string>? visibility)
    {
        var normalized = (visibility ?? [])
            .Select(Clean)
            .Where(key => key != null)
            .Select(key => key!.ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return normalized.Any(key => !AllowedVisibility.Contains(key))
            ? ServiceResult<string[]>.Fail("Visibilità non valida.")
            : ServiceResult<string[]>.Ok(normalized);
    }

    private static UtenteDipendenteAdminDto MapDipendente(Dipendenti dipendente)
    {
        var user = dipendente.Utentis
            .OrderByDescending(u => u.Attivo)
            .ThenByDescending(u => u.Id)
            .FirstOrDefault();

        if (user == null)
        {
            return new UtenteDipendenteAdminDto
            {
                DipendenteId = dipendente.Id,
                DipendenteNome = dipendente.Nome,
                DipendenteCognome = dipendente.Cognome,
                DipendenteEmail = dipendente.Email,
                DipendenteMatricola = dipendente.Matricola,
                DipendenteAttivo = dipendente.Attivo,
                Roles = [],
                Visibility = []
            };
        }

        return MapUser(user);
    }

    private static UtenteDipendenteAdminDto MapUser(Utenti user)
    {
        var dipendente = user.Dipendente!;

        return new UtenteDipendenteAdminDto
        {
            DipendenteId = dipendente.Id,
            DipendenteNome = dipendente.Nome,
            DipendenteCognome = dipendente.Cognome,
            DipendenteEmail = dipendente.Email,
            DipendenteMatricola = dipendente.Matricola,
            DipendenteAttivo = dipendente.Attivo,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccountAttivo = user.Attivo,
            Roles = user.UtentiRuolis
                .Where(ur => ur.Ruolo.Attivo)
                .Select(ur => ur.Ruolo.Nome.Trim().ToUpperInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(role => role)
                .ToArray(),
            Visibility = user.UtentiVisibilitas
                .Select(v => v.Chiave.Trim().ToLowerInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(key => key)
                .ToArray(),
            LastLoginAtUtc = user.UltimoAccessoAt
        };
    }

    private static bool UserHasAdminRole(Utenti user)
    {
        return user.UtentiRuolis.Any(ur =>
            ur.Ruolo.Attivo &&
            string.Equals(ur.Ruolo.Nome, RoleCodes.Admin, StringComparison.OrdinalIgnoreCase));
    }

    private static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string symbols = "!@$%?";
        const string all = upper + lower + digits + symbols;

        var chars = new char[14];
        chars[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        chars[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        chars[3] = symbols[RandomNumberGenerator.GetInt32(symbols.Length)];

        for (var i = 4; i < chars.Length; i++)
        {
            chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        return new string(chars.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

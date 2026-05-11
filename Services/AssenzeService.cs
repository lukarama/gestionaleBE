using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gestionale.Api.Services;

public class AssenzeService : IAssenzeService
{
    private const string StatoRichiesto = "richiesto";
    private const string StatoApprovato = "approvato";
    private const string StatoRimandato = "rimandato";
    private const string StatoRifiutato = "rifiutato";

    private static readonly HashSet<string> StatiGestioneAdmin = new(StringComparer.OrdinalIgnoreCase)
    {
        StatoApprovato,
        StatoRimandato,
        StatoRifiutato
    };

    private readonly AppDbContext _context;

    public AssenzeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TipoAssenzaDto>> GetTipiAssenzaAsync()
    {
        return await _context.TipiAssenza
            .AsNoTracking()
            .Where(t => t.Attivo)
            .OrderBy(t => t.Nome)
            .Select(t => new TipoAssenzaDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Descrizione = t.Descrizione,
                Attivo = t.Attivo
            })
            .ToListAsync();
    }

    public async Task<List<DipendenteSelectDto>> GetDipendentiSelectAsync()
    {
        return await _context.Dipendentis
            .AsNoTracking()
            .Where(d => d.Attivo)
            .OrderBy(d => d.Nome)
            .ThenBy(d => d.Cognome)
            .Select(d => new DipendenteSelectDto
            {
                Id = d.Id,
                Label = d.Nome + " " + d.Cognome,
                NomeCompleto = d.Nome + " " + d.Cognome
            })
            .ToListAsync();
    }

    public async Task<ServiceResult<List<AssenzaListDto>>> GetMieRichiesteAsync(UserContext user)
    {
        var query = _context.Assenze
            .AsNoTracking()
            .Include(a => a.Dipendente)
            .Include(a => a.TipoAssenza)
            .AsQueryable();

        if (!user.IsAdmin)
        {
            if (!user.DipendenteId.HasValue)
            {
                return ServiceResult<List<AssenzaListDto>>.Fail("Dipendente non valido.", 403);
            }

            query = query.Where(a => a.DipendenteId == user.DipendenteId.Value);
        }

        var result = await query
            .OrderByDescending(a => a.DataRichiesta)
            .Select(ToListDtoExpression())
            .ToListAsync();

        return ServiceResult<List<AssenzaListDto>>.Ok(result);
    }

    public async Task<ServiceResult<AssenzaListDto>> CreateRichiestaAsync(CreateAssenzaDto dto, UserContext user)
    {
        var dipendenteId = user.IsAdmin ? dto.DipendenteId : user.DipendenteId.GetValueOrDefault();

        if (!user.IsAdmin && dipendenteId <= 0)
        {
            return ServiceResult<AssenzaListDto>.Fail("Dipendente non valido.", 403);
        }

        if (dipendenteId <= 0)
        {
            return ServiceResult<AssenzaListDto>.Fail("Dipendente non valido.");
        }

        var dataMinima = DateOnly.FromDateTime(DateTime.Today).AddDays(3);
        if (dto.DataInizio < dataMinima)
        {
            return ServiceResult<AssenzaListDto>.Fail("La data inizio deve essere almeno tra 3 giorni.");
        }

        if (dto.DataFine < dto.DataInizio)
        {
            return ServiceResult<AssenzaListDto>.Fail("La data fine non può essere precedente alla data inizio.");
        }

        var tipoEsiste = await _context.TipiAssenza
            .AnyAsync(t => t.Id == dto.TipoAssenzaId && t.Attivo);

        if (!tipoEsiste)
        {
            return ServiceResult<AssenzaListDto>.Fail("Tipo assenza non valido.");
        }

        var dipendenteEsiste = await _context.Dipendentis
            .AnyAsync(d => d.Id == dipendenteId && d.Attivo);

        if (!dipendenteEsiste)
        {
            return ServiceResult<AssenzaListDto>.Fail("Dipendente non valido.");
        }

        var assenza = new Assenza
        {
            DipendenteId = dipendenteId,
            TipoAssenzaId = dto.TipoAssenzaId,
            DataInizio = dto.DataInizio,
            DataFine = dto.DataFine,
            Giorni = dto.DataFine.DayNumber - dto.DataInizio.DayNumber + 1,
            Note = Pulisci(dto.Note),
            DataRichiesta = DateTime.UtcNow,
            Stato = StatoRichiesto,
            CreatedAt = DateTime.UtcNow
        };

        _context.Assenze.Add(assenza);
        await _context.SaveChangesAsync();

        var created = await GetByIdAsync(assenza.Id);
        return ServiceResult<AssenzaListDto>.Created(created!);
    }

    public async Task<ServiceResult<bool>> DeleteRichiestaAsync(int id, UserContext user)
    {
        var assenza = await _context.Assenze.FirstOrDefaultAsync(a => a.Id == id);

        if (assenza == null)
        {
            return ServiceResult<bool>.Fail("Richiesta assenza non trovata.", 404);
        }

        if (!user.IsAdmin && assenza.DipendenteId != user.DipendenteId)
        {
            return ServiceResult<bool>.Fail("Non autorizzato.", 403);
        }

        if (!string.Equals(assenza.Stato, StatoRichiesto, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<bool>.Fail("La richiesta può essere eliminata solo se è ancora in stato richiesto.");
        }

        _context.Assenze.Remove(assenza);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<AssenzaListDto>> UpdateStatoRichiestaAsync(int id, UpdateStatoAssenzaDto dto, UserContext user)
    {
        if (!user.IsAdmin)
        {
            return ServiceResult<AssenzaListDto>.Fail("Non autorizzato.", 403);
        }

        var stato = Pulisci(dto.Stato)?.ToLowerInvariant();
        if (stato == null || !StatiGestioneAdmin.Contains(stato))
        {
            return ServiceResult<AssenzaListDto>.Fail("Stato richiesta non valido.");
        }

        var note = Pulisci(dto.Note);
        if (string.Equals(stato, StatoRimandato, StringComparison.OrdinalIgnoreCase) && note == null)
        {
            return ServiceResult<AssenzaListDto>.Fail("Inserisci una nota per rimandare la richiesta.");
        }

        if (note?.Length > 1000)
        {
            return ServiceResult<AssenzaListDto>.Fail("La nota non puo superare 1000 caratteri.");
        }

        var assenza = await _context.Assenze.FirstOrDefaultAsync(a => a.Id == id);
        if (assenza == null)
        {
            return ServiceResult<AssenzaListDto>.Fail("Richiesta assenza non trovata.", 404);
        }

        assenza.Stato = stato;
        if (string.Equals(stato, StatoRimandato, StringComparison.OrdinalIgnoreCase))
        {
            assenza.Note = note;
        }

        assenza.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var updated = await GetByIdAsync(assenza.Id);
        return ServiceResult<AssenzaListDto>.Ok(updated!);
    }

    private async Task<AssenzaListDto?> GetByIdAsync(int id)
    {
        return await _context.Assenze
            .AsNoTracking()
            .Include(a => a.Dipendente)
            .Include(a => a.TipoAssenza)
            .Where(a => a.Id == id)
            .Select(ToListDtoExpression())
            .FirstOrDefaultAsync();
    }

    private static Expression<Func<Assenza, AssenzaListDto>> ToListDtoExpression()
    {
        return assenza => new AssenzaListDto
        {
            Id = assenza.Id,
            DipendenteId = assenza.DipendenteId,
            Dipendente = assenza.Dipendente.Nome + " " + assenza.Dipendente.Cognome,
            TipoAssenzaId = assenza.TipoAssenzaId,
            TipoAssenza = assenza.TipoAssenza.Nome,
            DataInizio = assenza.DataInizio,
            DataFine = assenza.DataFine,
            Giorni = assenza.Giorni,
            Stato = assenza.Stato,
            Note = assenza.Note,
            DataRichiesta = assenza.DataRichiesta
        };
    }

    private static string? Pulisci(string? valore)
    {
        return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
    }
}

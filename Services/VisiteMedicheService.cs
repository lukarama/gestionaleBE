using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class VisiteMedicheService : IVisiteMedicheService
{
    private readonly AppDbContext _context;

    public VisiteMedicheService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VisitaMedicaListDto>> GetAllAsync()
    {
        return await _context.VisiteMediches
            .AsNoTracking()
            .Include(v => v.Dipendente)
            .Include(v => v.TipoVisitaMedica)
            .Include(v => v.EsitoVisitaMedica)
            .OrderByDescending(v => v.DataVisita)
            .ThenByDescending(v => v.Id)
            .Select(v => new VisitaMedicaListDto
            {
                Id = v.Id,
                DipendenteId = v.DipendenteId,
                Dipendente = v.Dipendente.Cognome + " " + v.Dipendente.Nome,
                TipoVisitaMedicaId = v.TipoVisitaMedicaId,
                TipoVisitaMedica = v.TipoVisitaMedica.Nome,
                DataVisita = v.DataVisita,
                DataScadenza = v.DataScadenza,
                EsitoVisitaMedicaId = v.EsitoVisitaMedicaId,
                EsitoVisitaMedica = v.EsitoVisitaMedica != null ? v.EsitoVisitaMedica.Nome : null,
                Idoneo = v.Idoneo,
                MedicoCompetente = v.MedicoCompetente,
                StrutturaSanitaria = v.StrutturaSanitaria
            })
            .ToListAsync();
    }

    public async Task<List<VisitaMedicaScadenzaDashboardDto>> GetDashboardScadenzeAsync(int giorni = 30)
    {
        if (giorni <= 0)
            giorni = 30;

        var oggi = DateOnly.FromDateTime(DateTime.Today);
        var dataLimite = oggi.AddDays(giorni);

        return await _context.VisiteMediches
            .AsNoTracking()
            .Include(v => v.Dipendente)
            .Include(v => v.TipoVisitaMedica)
            .Include(v => v.EsitoVisitaMedica)
            .Where(v => v.DataScadenza.HasValue && v.DataScadenza.Value <= dataLimite)
            .OrderBy(v => v.DataScadenza)
            .ThenBy(v => v.Dipendente.Cognome)
            .ThenBy(v => v.Dipendente.Nome)
            .Select(v => new VisitaMedicaScadenzaDashboardDto
            {
                Id = v.Id,
                DipendenteId = v.DipendenteId,
                Dipendente = v.Dipendente.Cognome + " " + v.Dipendente.Nome,
                TipoVisitaMedicaId = v.TipoVisitaMedicaId,
                TipoVisitaMedica = v.TipoVisitaMedica.Nome,
                DataVisita = v.DataVisita,
                DataScadenza = v.DataScadenza,
                GiorniAllaScadenza = v.DataScadenza.HasValue
                    ? v.DataScadenza.Value.DayNumber - oggi.DayNumber
                    : 0,
                Scaduta = v.DataScadenza.HasValue && v.DataScadenza.Value < oggi,
                InScadenza = v.DataScadenza.HasValue && v.DataScadenza.Value >= oggi && v.DataScadenza.Value <= dataLimite,
                Idoneo = v.Idoneo,
                EsitoVisitaMedica = v.EsitoVisitaMedica != null ? v.EsitoVisitaMedica.Nome : null
            })
            .ToListAsync();
    }

    public async Task<VisitaMedicaDetailDto?> GetByIdAsync(int id)
    {
        return await _context.VisiteMediches
            .AsNoTracking()
            .Include(v => v.Dipendente)
            .Include(v => v.TipoVisitaMedica)
            .Include(v => v.EsitoVisitaMedica)
            .Where(v => v.Id == id)
            .Select(v => new VisitaMedicaDetailDto
            {
                Id = v.Id,
                DipendenteId = v.DipendenteId,
                Dipendente = v.Dipendente.Cognome + " " + v.Dipendente.Nome,
                TipoVisitaMedicaId = v.TipoVisitaMedicaId,
                TipoVisitaMedica = v.TipoVisitaMedica.Nome,
                DataVisita = v.DataVisita,
                DataScadenza = v.DataScadenza,
                EsitoVisitaMedicaId = v.EsitoVisitaMedicaId,
                EsitoVisitaMedica = v.EsitoVisitaMedica != null ? v.EsitoVisitaMedica.Nome : null,
                Idoneo = v.Idoneo,
                Prescrizioni = v.Prescrizioni,
                MedicoCompetente = v.MedicoCompetente,
                StrutturaSanitaria = v.StrutturaSanitaria,
                Note = v.Note,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<VisitaMedicaDetailDto>> CreateAsync(CreateVisitaMedicaDto dto)
    {
        var validazione = await ValidaRelazioniAsync(
            dto.DipendenteId,
            dto.TipoVisitaMedicaId,
            dto.EsitoVisitaMedicaId);

        if (!validazione.Success)
            return ServiceResult<VisitaMedicaDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        if (dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataVisita)
            return ServiceResult<VisitaMedicaDetailDto>.Fail("La data di scadenza non può essere precedente alla data visita.", 400);

        var entity = new VisiteMediche
        {
            DipendenteId = dto.DipendenteId,
            TipoVisitaMedicaId = dto.TipoVisitaMedicaId,
            DataVisita = dto.DataVisita,
            DataScadenza = dto.DataScadenza,
            EsitoVisitaMedicaId = dto.EsitoVisitaMedicaId,
            Idoneo = dto.Idoneo,
            Prescrizioni = Pulisci(dto.Prescrizioni),
            MedicoCompetente = Pulisci(dto.MedicoCompetente),
            StrutturaSanitaria = Pulisci(dto.StrutturaSanitaria),
            Note = Pulisci(dto.Note),
            CreatedAt = DateTime.Now
        };

        _context.VisiteMediches.Add(entity);
        await _context.SaveChangesAsync();

        var result = await GetByIdAsync(entity.Id);
        return ServiceResult<VisitaMedicaDetailDto>.Created(result!);
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateVisitaMedicaDto dto)
    {
        var entity = await _context.VisiteMediches
            .FirstOrDefaultAsync(v => v.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Visita medica non trovata.", 404);

        var validazione = await ValidaRelazioniAsync(
            dto.DipendenteId,
            dto.TipoVisitaMedicaId,
            dto.EsitoVisitaMedicaId);

        if (!validazione.Success)
            return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

        if (dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataVisita)
            return ServiceResult<bool>.Fail("La data di scadenza non può essere precedente alla data visita.", 400);

        entity.DipendenteId = dto.DipendenteId;
        entity.TipoVisitaMedicaId = dto.TipoVisitaMedicaId;
        entity.DataVisita = dto.DataVisita;
        entity.DataScadenza = dto.DataScadenza;
        entity.EsitoVisitaMedicaId = dto.EsitoVisitaMedicaId;
        entity.Idoneo = dto.Idoneo;
        entity.Prescrizioni = Pulisci(dto.Prescrizioni);
        entity.MedicoCompetente = Pulisci(dto.MedicoCompetente);
        entity.StrutturaSanitaria = Pulisci(dto.StrutturaSanitaria);
        entity.Note = Pulisci(dto.Note);
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var entity = await _context.VisiteMediches
            .FirstOrDefaultAsync(v => v.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Visita medica non trovata.", 404);

        _context.VisiteMediches.Remove(entity);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<List<VisitaMedicaListDto>> GetByDipendenteIdAsync(int dipendenteId)
    {
        return await _context.VisiteMediches
            .AsNoTracking()
            .Include(v => v.Dipendente)
            .Include(v => v.TipoVisitaMedica)
            .Include(v => v.EsitoVisitaMedica)
            .Where(v => v.DipendenteId == dipendenteId)
            .OrderByDescending(v => v.DataVisita)
            .ThenByDescending(v => v.Id)
            .Select(v => new VisitaMedicaListDto
            {
                Id = v.Id,
                DipendenteId = v.DipendenteId,
                Dipendente = v.Dipendente.Cognome + " " + v.Dipendente.Nome,
                TipoVisitaMedicaId = v.TipoVisitaMedicaId,
                TipoVisitaMedica = v.TipoVisitaMedica.Nome,
                DataVisita = v.DataVisita,
                DataScadenza = v.DataScadenza,
                EsitoVisitaMedicaId = v.EsitoVisitaMedicaId,
                EsitoVisitaMedica = v.EsitoVisitaMedica != null ? v.EsitoVisitaMedica.Nome : null,
                Idoneo = v.Idoneo,
                MedicoCompetente = v.MedicoCompetente,
                StrutturaSanitaria = v.StrutturaSanitaria
            })
            .ToListAsync();
    }

    public async Task<List<VisitaMedicaListDto>> GetInScadenzaAsync(int giorni = 30)
    {
        if (giorni <= 0)
            giorni = 30;

        var oggi = DateOnly.FromDateTime(DateTime.Today);
        var scadenzaMax = oggi.AddDays(giorni);

        return await _context.VisiteMediches
            .AsNoTracking()
            .Include(v => v.Dipendente)
            .Include(v => v.TipoVisitaMedica)
            .Include(v => v.EsitoVisitaMedica)
            .Where(v => v.DataScadenza.HasValue && v.DataScadenza.Value >= oggi && v.DataScadenza.Value <= scadenzaMax)
            .OrderBy(v => v.DataScadenza)
            .ThenBy(v => v.Dipendente.Cognome)
            .ThenBy(v => v.Dipendente.Nome)
            .Select(v => new VisitaMedicaListDto
            {
                Id = v.Id,
                DipendenteId = v.DipendenteId,
                Dipendente = v.Dipendente.Cognome + " " + v.Dipendente.Nome,
                TipoVisitaMedicaId = v.TipoVisitaMedicaId,
                TipoVisitaMedica = v.TipoVisitaMedica.Nome,
                DataVisita = v.DataVisita,
                DataScadenza = v.DataScadenza,
                EsitoVisitaMedicaId = v.EsitoVisitaMedicaId,
                EsitoVisitaMedica = v.EsitoVisitaMedica != null ? v.EsitoVisitaMedica.Nome : null,
                Idoneo = v.Idoneo,
                MedicoCompetente = v.MedicoCompetente,
                StrutturaSanitaria = v.StrutturaSanitaria
            })
            .ToListAsync();
    }

    public async Task<List<TipoVisitaMedicaSelectDto>> GetTipiVisitaSelectAsync()
    {
        return await _context.TipiVisitaMedicas
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .Select(t => new TipoVisitaMedicaSelectDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Label = t.Nome
            })
            .ToListAsync();
    }

    public async Task<List<EsitoVisitaMedicaSelectDto>> GetEsitiSelectAsync()
    {
        return await _context.EsitiVisitaMedicas
            .AsNoTracking()
            .OrderBy(e => e.Nome)
            .Select(e => new EsitoVisitaMedicaSelectDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Label = e.Nome
            })
            .ToListAsync();
    }

    private async Task<ServiceResult<bool>> ValidaRelazioniAsync(
        int dipendenteId,
        int tipoVisitaMedicaId,
        int? esitoVisitaMedicaId)
    {
        var dipendenteEsiste = await _context.Dipendentis
            .AnyAsync(d => d.Id == dipendenteId && d.Attivo);

        if (!dipendenteEsiste)
            return ServiceResult<bool>.Fail("Il dipendente selezionato non esiste o non è attivo.", 400);

        var tipoEsiste = await _context.TipiVisitaMedicas
            .AnyAsync(t => t.Id == tipoVisitaMedicaId);

        if (!tipoEsiste)
            return ServiceResult<bool>.Fail("Il tipo visita medica selezionato non esiste.", 400);

        if (esitoVisitaMedicaId.HasValue)
        {
            var esitoEsiste = await _context.EsitiVisitaMedicas
                .AnyAsync(e => e.Id == esitoVisitaMedicaId.Value);

            if (!esitoEsiste)
                return ServiceResult<bool>.Fail("L'esito visita medica selezionato non esiste.", 400);
        }

        return ServiceResult<bool>.Ok(true);
    }

    private static string? Pulisci(string? valore)
    {
        return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
    }
}
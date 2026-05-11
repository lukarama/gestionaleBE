using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class AssegnazioniDpiService : IAssegnazioniDpiService
    {
        private readonly AppDbContext _context;

        public AssegnazioniDpiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AssegnazioneDpiListDto>> GetAllAsync()
        {
            return await _context.AssegnazioniDpis
                .Include(a => a.Dipendente)
                .Include(a => a.Dpi)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .OrderByDescending(a => a.DataConsegna)
                .Select(a => new AssegnazioneDpiListDto
                {
                    Id = a.Id,
                    DipendenteId = a.DipendenteId,
                    Dipendente = a.Dipendente.Cognome + " " + a.Dipendente.Nome,
                    DpiId = a.DpiId,
                    Dpi = a.Dpi.Nome,
                    CantiereId = a.CantiereId,
                    Cantiere = a.Cantiere != null ? a.Cantiere.Nome : null,
                    Quantita = a.Quantita,
                    StatoAssegnazioneId = a.StatoAssegnazioneId,
                    StatoAssegnazione = a.StatoAssegnazione.Nome,
                    DataConsegna = a.DataConsegna,
                    DataScadenza = a.DataScadenza,
                    DataRestituzione = a.DataRestituzione,
                    FirmaConsegna = a.FirmaConsegna,
                    Note = a.Note
                })
                .ToListAsync();
        }

        public async Task<AssegnazioneDpiDetailDto?> GetByIdAsync(int id)
        {
            return await _context.AssegnazioniDpis
                .Include(a => a.Dipendente)
                .Include(a => a.Dpi)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .Where(a => a.Id == id)
                .Select(a => new AssegnazioneDpiDetailDto
                {
                    Id = a.Id,
                    DipendenteId = a.DipendenteId,
                    Dipendente = a.Dipendente.Cognome + " " + a.Dipendente.Nome,
                    DpiId = a.DpiId,
                    Dpi = a.Dpi.Nome,
                    CantiereId = a.CantiereId,
                    Cantiere = a.Cantiere != null ? a.Cantiere.Nome : null,
                    Quantita = a.Quantita,
                    StatoAssegnazioneId = a.StatoAssegnazioneId,
                    StatoAssegnazione = a.StatoAssegnazione.Nome,
                    DataConsegna = a.DataConsegna,
                    DataScadenza = a.DataScadenza,
                    DataRestituzione = a.DataRestituzione,
                    FirmaConsegna = a.FirmaConsegna,
                    Note = a.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<AssegnazioneDpiListDto>> GetByDipendenteIdAsync(int dipendenteId)
        {
            return await _context.AssegnazioniDpis
                .AsNoTracking()
                .Include(a => a.Dipendente)
                .Include(a => a.Dpi)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .Where(a => a.DipendenteId == dipendenteId)
                .OrderByDescending(a => a.DataConsegna)
                .ThenByDescending(a => a.Id)
                .Select(a => new AssegnazioneDpiListDto
                {
                    Id = a.Id,
                    DipendenteId = a.DipendenteId,
                    Dipendente = a.Dipendente.Cognome + " " + a.Dipendente.Nome,
                    DpiId = a.DpiId,
                    Dpi = a.Dpi.Nome,
                    CantiereId = a.CantiereId,
                    Cantiere = a.Cantiere != null ? a.Cantiere.Nome : null,
                    Quantita = a.Quantita,
                    StatoAssegnazioneId = a.StatoAssegnazioneId,
                    StatoAssegnazione = a.StatoAssegnazione.Nome,
                    DataConsegna = a.DataConsegna,
                    DataScadenza = a.DataScadenza,
                    DataRestituzione = a.DataRestituzione,
                    FirmaConsegna = a.FirmaConsegna,
                    Note = a.Note
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<AssegnazioneDpiDetailDto>> CreateAsync(CreateAssegnazioneDpiDto dto)
        {
            var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.DpiId, dto.CantiereId, dto.StatoAssegnazioneId);
            if (!validazione.Success)
                return ServiceResult<AssegnazioneDpiDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            if (dto.DataRestituzione.HasValue && dto.DataRestituzione.Value < dto.DataConsegna)
                return ServiceResult<AssegnazioneDpiDetailDto>.Fail("La data di restituzione non può essere precedente alla data di consegna.", 400);

            if (dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataConsegna)
                return ServiceResult<AssegnazioneDpiDetailDto>.Fail("La data di scadenza non può essere precedente alla data di consegna.", 400);

            var assegnazione = new AssegnazioniDpi
            {
                DipendenteId = dto.DipendenteId,
                DpiId = dto.DpiId,
                CantiereId = dto.CantiereId,
                Quantita = dto.Quantita,
                StatoAssegnazioneId = dto.StatoAssegnazioneId,
                DataConsegna = dto.DataConsegna,
                DataScadenza = dto.DataScadenza,
                DataRestituzione = dto.DataRestituzione,
                FirmaConsegna = dto.FirmaConsegna,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.AssegnazioniDpis.Add(assegnazione);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(assegnazione.Id);
            return ServiceResult<AssegnazioneDpiDetailDto>.Created(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateAssegnazioneDpiDto dto)
        {
            var assegnazione = await _context.AssegnazioniDpis
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assegnazione == null)
                return ServiceResult<bool>.Fail("Assegnazione DPI non trovata.", 404);

            var validazione = await ValidaRelazioniAsync(dto.DipendenteId, dto.DpiId, dto.CantiereId, dto.StatoAssegnazioneId);
            if (!validazione.Success)
                return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            if (dto.DataRestituzione.HasValue && dto.DataRestituzione.Value < dto.DataConsegna)
                return ServiceResult<bool>.Fail("La data di restituzione non può essere precedente alla data di consegna.", 400);

            if (dto.DataScadenza.HasValue && dto.DataScadenza.Value < dto.DataConsegna)
                return ServiceResult<bool>.Fail("La data di scadenza non può essere precedente alla data di consegna.", 400);

            assegnazione.DipendenteId = dto.DipendenteId;
            assegnazione.DpiId = dto.DpiId;
            assegnazione.CantiereId = dto.CantiereId;
            assegnazione.Quantita = dto.Quantita;
            assegnazione.StatoAssegnazioneId = dto.StatoAssegnazioneId;
            assegnazione.DataConsegna = dto.DataConsegna;
            assegnazione.DataScadenza = dto.DataScadenza;
            assegnazione.DataRestituzione = dto.DataRestituzione;
            assegnazione.FirmaConsegna = dto.FirmaConsegna;
            assegnazione.Note = Pulisci(dto.Note);
            assegnazione.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var assegnazione = await _context.AssegnazioniDpis
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assegnazione == null)
                return ServiceResult<bool>.Fail("Assegnazione DPI non trovata.", 404);

            _context.AssegnazioniDpis.Remove(assegnazione);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaRelazioniAsync(
            int dipendenteId,
            int dpiId,
            int? cantiereId,
            int statoAssegnazioneId)
        {
            var dipendenteEsiste = await _context.Dipendentis
                .AnyAsync(d => d.Id == dipendenteId);

            if (!dipendenteEsiste)
                return ServiceResult<bool>.Fail("Il dipendente selezionato non esiste.", 400);

            var dpiEsiste = await _context.Dpis
                .AnyAsync(d => d.Id == dpiId);

            if (!dpiEsiste)
                return ServiceResult<bool>.Fail("Il DPI selezionato non esiste.", 400);

            if (cantiereId.HasValue)
            {
                var cantiereEsiste = await _context.Cantieris
                    .AnyAsync(c => c.Id == cantiereId.Value);

                if (!cantiereEsiste)
                    return ServiceResult<bool>.Fail("Il cantiere selezionato non esiste.", 400);
            }

            var statoEsiste = await _context.StatiAssegnaziones
                .AnyAsync(s => s.Id == statoAssegnazioneId);

            if (!statoEsiste)
                return ServiceResult<bool>.Fail("Lo stato assegnazione selezionato non esiste.", 400);

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}

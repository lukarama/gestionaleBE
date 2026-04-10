using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class AssegnazioniMezziService : IAssegnazioniMezziService
    {
        private readonly AppDbContext _context;

        public AssegnazioniMezziService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AssegnazioneMezzoListDto>> GetAllAsync()
        {
            return await _context.AssegnazioniMezzis
                .Include(a => a.Mezzo)
                .Include(a => a.Dipendente)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .OrderByDescending(a => a.DataInizio)
                .Select(a => new AssegnazioneMezzoListDto
                {
                    Id = a.Id,
                    MezzoId = a.MezzoId,
                    Mezzo = (a.Mezzo.Targa ?? "-") + " - " + (a.Mezzo.Marca ?? "") + " " + (a.Mezzo.Modello ?? ""),
                    DipendenteId = a.DipendenteId,
                    Dipendente = a.Dipendente != null ? a.Dipendente.Cognome + " " + a.Dipendente.Nome : null,
                    CantiereId = a.CantiereId,
                    Cantiere = a.Cantiere != null ? a.Cantiere.Nome : null,
                    StatoAssegnazioneId = a.StatoAssegnazioneId,
                    StatoAssegnazione = a.StatoAssegnazione.Nome,
                    DataInizio = a.DataInizio,
                    DataFine = a.DataFine,
                    KmConsegna = a.KmConsegna,
                    KmRientro = a.KmRientro
                })
                .ToListAsync();
        }

        public async Task<AssegnazioneMezzoDetailDto?> GetByIdAsync(int id)
        {
            return await _context.AssegnazioniMezzis
                .Include(a => a.Mezzo)
                .Include(a => a.Dipendente)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .Where(a => a.Id == id)
                .Select(a => new AssegnazioneMezzoDetailDto
                {
                    Id = a.Id,
                    MezzoId = a.MezzoId,
                    Mezzo = (a.Mezzo.Targa ?? "-") + " - " + (a.Mezzo.Marca ?? "") + " " + (a.Mezzo.Modello ?? ""),
                    DipendenteId = a.DipendenteId,
                    Dipendente = a.Dipendente != null ? a.Dipendente.Cognome + " " + a.Dipendente.Nome : null,
                    CantiereId = a.CantiereId,
                    Cantiere = a.Cantiere != null ? a.Cantiere.Nome : null,
                    StatoAssegnazioneId = a.StatoAssegnazioneId,
                    StatoAssegnazione = a.StatoAssegnazione.Nome,
                    DataInizio = a.DataInizio,
                    DataFine = a.DataFine,
                    KmConsegna = a.KmConsegna,
                    KmRientro = a.KmRientro,
                    Note = a.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<AssegnazioneMezzoDetailDto>> CreateAsync(CreateAssegnazioneMezzoDto dto)
        {
            var validazione = await ValidaRelazioniAsync(dto.MezzoId, dto.DipendenteId, dto.CantiereId, dto.StatoAssegnazioneId);
            if (!validazione.Success)
                return ServiceResult<AssegnazioneMezzoDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            var validazioneDateKm = ValidaDateEChilometri(dto.DataInizio, dto.DataFine, dto.KmConsegna, dto.KmRientro);
            if (!validazioneDateKm.Success)
                return ServiceResult<AssegnazioneMezzoDetailDto>.Fail(validazioneDateKm.Message!, validazioneDateKm.StatusCode ?? 400);

            var conflitto = await _context.AssegnazioniMezzis.AnyAsync(a =>
                a.MezzoId == dto.MezzoId &&
                (
                    !a.DataFine.HasValue ||
                    a.DataFine.Value >= dto.DataInizio
                ));

            if (conflitto)
                return ServiceResult<AssegnazioneMezzoDetailDto>.Fail("Il mezzo risulta già assegnato in un periodo compatibile con questa assegnazione.", 400);

            var assegnazione = new AssegnazioniMezzi
            {
                MezzoId = dto.MezzoId,
                DipendenteId = dto.DipendenteId,
                CantiereId = dto.CantiereId,
                StatoAssegnazioneId = dto.StatoAssegnazioneId,
                DataInizio = dto.DataInizio,
                DataFine = dto.DataFine,
                KmConsegna = dto.KmConsegna,
                KmRientro = dto.KmRientro,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.AssegnazioniMezzis.Add(assegnazione);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(assegnazione.Id);
            return ServiceResult<AssegnazioneMezzoDetailDto>.Created(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateAssegnazioneMezzoDto dto)
        {
            var assegnazione = await _context.AssegnazioniMezzis
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assegnazione == null)
                return ServiceResult<bool>.Fail("Assegnazione mezzo non trovata.", 404);

            var validazione = await ValidaRelazioniAsync(dto.MezzoId, dto.DipendenteId, dto.CantiereId, dto.StatoAssegnazioneId);
            if (!validazione.Success)
                return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            var validazioneDateKm = ValidaDateEChilometri(dto.DataInizio, dto.DataFine, dto.KmConsegna, dto.KmRientro);
            if (!validazioneDateKm.Success)
                return ServiceResult<bool>.Fail(validazioneDateKm.Message!, validazioneDateKm.StatusCode ?? 400);

            var conflitto = await _context.AssegnazioniMezzis.AnyAsync(a =>
                a.Id != id &&
                a.MezzoId == dto.MezzoId &&
                (
                    !a.DataFine.HasValue ||
                    a.DataFine.Value >= dto.DataInizio
                ));

            if (conflitto)
                return ServiceResult<bool>.Fail("Il mezzo risulta già assegnato in un periodo compatibile con questa assegnazione.", 400);

            assegnazione.MezzoId = dto.MezzoId;
            assegnazione.DipendenteId = dto.DipendenteId;
            assegnazione.CantiereId = dto.CantiereId;
            assegnazione.StatoAssegnazioneId = dto.StatoAssegnazioneId;
            assegnazione.DataInizio = dto.DataInizio;
            assegnazione.DataFine = dto.DataFine;
            assegnazione.KmConsegna = dto.KmConsegna;
            assegnazione.KmRientro = dto.KmRientro;
            assegnazione.Note = Pulisci(dto.Note);
            assegnazione.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var assegnazione = await _context.AssegnazioniMezzis
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assegnazione == null)
                return ServiceResult<bool>.Fail("Assegnazione mezzo non trovata.", 404);

            _context.AssegnazioniMezzis.Remove(assegnazione);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaRelazioniAsync(
            int mezzoId,
            int? dipendenteId,
            int? cantiereId,
            int statoAssegnazioneId)
        {
            var mezzoEsiste = await _context.Mezzis
                .AnyAsync(m => m.Id == mezzoId);

            if (!mezzoEsiste)
                return ServiceResult<bool>.Fail("Il mezzo selezionato non esiste.", 400);

            if (dipendenteId.HasValue)
            {
                var dipendenteEsiste = await _context.Dipendentis
                    .AnyAsync(d => d.Id == dipendenteId.Value);

                if (!dipendenteEsiste)
                    return ServiceResult<bool>.Fail("Il dipendente selezionato non esiste.", 400);
            }

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

        private static ServiceResult<bool> ValidaDateEChilometri(
            DateTime dataInizio,
            DateTime? dataFine,
            int? kmConsegna,
            int? kmRientro)
        {
            if (dataFine.HasValue && dataFine.Value < dataInizio)
                return ServiceResult<bool>.Fail("La data fine non può essere precedente alla data inizio.", 400);

            if (kmConsegna.HasValue && kmConsegna.Value < 0)
                return ServiceResult<bool>.Fail("I km consegna non possono essere negativi.", 400);

            if (kmRientro.HasValue && kmRientro.Value < 0)
                return ServiceResult<bool>.Fail("I km rientro non possono essere negativi.", 400);

            if (kmConsegna.HasValue && kmRientro.HasValue && kmRientro.Value < kmConsegna.Value)
                return ServiceResult<bool>.Fail("I km rientro non possono essere inferiori ai km consegna.", 400);

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
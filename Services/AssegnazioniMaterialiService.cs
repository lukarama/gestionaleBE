using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class AssegnazioniMaterialiService : IAssegnazioniMaterialiService
    {
        private readonly AppDbContext _context;

        public AssegnazioniMaterialiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AssegnazioneMaterialeListDto>> GetAllAsync()
        {
            return await _context.AssegnazioniMaterialis
                .Include(a => a.Materiale)
                .Include(a => a.Dipendente)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .OrderByDescending(a => a.DataAssegnazione)
                .Select(a => new AssegnazioneMaterialeListDto
                {
                    Id = a.Id,
                    MaterialeId = a.MaterialeId,
                    Materiale = a.Materiale.Nome,
                    DipendenteId = a.DipendenteId,
                    Dipendente = a.Dipendente != null ? a.Dipendente.Cognome + " " + a.Dipendente.Nome : null,
                    CantiereId = a.CantiereId,
                    Cantiere = a.Cantiere != null ? a.Cantiere.Nome : null,
                    Quantita = a.Quantita,
                    StatoAssegnazioneId = a.StatoAssegnazioneId,
                    StatoAssegnazione = a.StatoAssegnazione.Nome,
                    DataAssegnazione = a.DataAssegnazione,
                    DataRestituzione = a.DataRestituzione
                })
                .ToListAsync();
        }

        public async Task<AssegnazioneMaterialeDetailDto?> GetByIdAsync(int id)
        {
            return await _context.AssegnazioniMaterialis
                .Include(a => a.Materiale)
                .Include(a => a.Dipendente)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .Where(a => a.Id == id)
                .Select(a => new AssegnazioneMaterialeDetailDto
                {
                    Id = a.Id,
                    MaterialeId = a.MaterialeId,
                    Materiale = a.Materiale.Nome,
                    DipendenteId = a.DipendenteId,
                    Dipendente = a.Dipendente != null ? a.Dipendente.Cognome + " " + a.Dipendente.Nome : null,
                    CantiereId = a.CantiereId,
                    Cantiere = a.Cantiere != null ? a.Cantiere.Nome : null,
                    Quantita = a.Quantita,
                    StatoAssegnazioneId = a.StatoAssegnazioneId,
                    StatoAssegnazione = a.StatoAssegnazione.Nome,
                    DataAssegnazione = a.DataAssegnazione,
                    DataRestituzione = a.DataRestituzione,
                    Note = a.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<AssegnazioneMaterialeDetailDto>> CreateAsync(CreateAssegnazioneMaterialeDto dto)
        {
            var validazione = await ValidaRelazioniAsync(dto.MaterialeId, dto.DipendenteId, dto.CantiereId, dto.StatoAssegnazioneId);
            if (!validazione.Success)
                return ServiceResult<AssegnazioneMaterialeDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            if (dto.DataRestituzione.HasValue && dto.DataRestituzione.Value < dto.DataAssegnazione)
                return ServiceResult<AssegnazioneMaterialeDetailDto>.Fail("La data di restituzione non può essere precedente alla data di assegnazione.", 400);

            var materiale = await _context.Materialis.FirstAsync(m => m.Id == dto.MaterialeId);

            if (materiale.QuantitaAttuale < dto.Quantita)
                return ServiceResult<AssegnazioneMaterialeDetailDto>.Fail("Quantità insufficiente a magazzino per questo materiale.", 400);

            var assegnazione = new AssegnazioniMateriali
            {
                MaterialeId = dto.MaterialeId,
                DipendenteId = dto.DipendenteId,
                CantiereId = dto.CantiereId,
                Quantita = dto.Quantita,
                StatoAssegnazioneId = dto.StatoAssegnazioneId,
                DataAssegnazione = dto.DataAssegnazione,
                DataRestituzione = dto.DataRestituzione,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            materiale.QuantitaAttuale -= dto.Quantita;

            _context.AssegnazioniMaterialis.Add(assegnazione);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(assegnazione.Id);
            return ServiceResult<AssegnazioneMaterialeDetailDto>.Created(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateAssegnazioneMaterialeDto dto)
        {
            var assegnazione = await _context.AssegnazioniMaterialis
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assegnazione == null)
                return ServiceResult<bool>.Fail("Assegnazione materiale non trovata.", 404);

            var validazione = await ValidaRelazioniAsync(dto.MaterialeId, dto.DipendenteId, dto.CantiereId, dto.StatoAssegnazioneId);
            if (!validazione.Success)
                return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            if (dto.DataRestituzione.HasValue && dto.DataRestituzione.Value < dto.DataAssegnazione)
                return ServiceResult<bool>.Fail("La data di restituzione non può essere precedente alla data di assegnazione.", 400);

            if (assegnazione.MaterialeId != dto.MaterialeId || assegnazione.Quantita != dto.Quantita)
            {
                var vecchioMateriale = await _context.Materialis.FirstAsync(m => m.Id == assegnazione.MaterialeId);
                vecchioMateriale.QuantitaAttuale += assegnazione.Quantita;

                var nuovoMateriale = await _context.Materialis.FirstAsync(m => m.Id == dto.MaterialeId);

                if (nuovoMateriale.QuantitaAttuale < dto.Quantita)
                    return ServiceResult<bool>.Fail("Quantità insufficiente a magazzino per questo materiale.", 400);

                nuovoMateriale.QuantitaAttuale -= dto.Quantita;
            }

            assegnazione.MaterialeId = dto.MaterialeId;
            assegnazione.DipendenteId = dto.DipendenteId;
            assegnazione.CantiereId = dto.CantiereId;
            assegnazione.Quantita = dto.Quantita;
            assegnazione.StatoAssegnazioneId = dto.StatoAssegnazioneId;
            assegnazione.DataAssegnazione = dto.DataAssegnazione;
            assegnazione.DataRestituzione = dto.DataRestituzione;
            assegnazione.Note = Pulisci(dto.Note);
            assegnazione.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var assegnazione = await _context.AssegnazioniMaterialis
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assegnazione == null)
                return ServiceResult<bool>.Fail("Assegnazione materiale non trovata.", 404);

            var materiale = await _context.Materialis.FirstAsync(m => m.Id == assegnazione.MaterialeId);
            materiale.QuantitaAttuale += assegnazione.Quantita;

            _context.AssegnazioniMaterialis.Remove(assegnazione);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaRelazioniAsync(
            int materialeId,
            int? dipendenteId,
            int? cantiereId,
            int statoAssegnazioneId)
        {
            var materialeEsiste = await _context.Materialis
                .AnyAsync(m => m.Id == materialeId);

            if (!materialeEsiste)
                return ServiceResult<bool>.Fail("Il materiale selezionato non esiste.", 400);

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

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
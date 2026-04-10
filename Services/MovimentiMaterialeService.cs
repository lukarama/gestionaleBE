using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class MovimentiMaterialeService : IMovimentiMaterialeService
    {
        private readonly AppDbContext _context;

        public MovimentiMaterialeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovimentoMaterialeListDto>> GetAllAsync()
        {
            return await _context.MovimentiMateriales
                .AsNoTracking()
                .Include(m => m.Materiale)
                .Include(m => m.TipoMovimentoMateriale)
                .Include(m => m.Dipendente)
                .Include(m => m.Cantiere)
                .OrderByDescending(m => m.DataMovimento)
                .ThenByDescending(m => m.Id)
                .Select(m => new MovimentoMaterialeListDto
                {
                    Id = m.Id,
                    MaterialeId = m.MaterialeId,
                    Materiale = m.Materiale.Nome,
                    TipoMovimentoMaterialeId = m.TipoMovimentoMaterialeId,
                    TipoMovimentoMateriale = m.TipoMovimentoMateriale.Nome,
                    Quantita = m.Quantita,
                    DataMovimento = m.DataMovimento,
                    DipendenteId = m.DipendenteId,
                    Dipendente = m.Dipendente != null ? m.Dipendente.Cognome + " " + m.Dipendente.Nome : null,
                    CantiereId = m.CantiereId,
                    Cantiere = m.Cantiere != null ? m.Cantiere.Nome : null,
                    RiferimentoTabella = m.RiferimentoTabella,
                    RiferimentoId = m.RiferimentoId
                })
                .ToListAsync();
        }

        public async Task<MovimentoMaterialeDetailDto?> GetByIdAsync(int id)
        {
            return await _context.MovimentiMateriales
                .AsNoTracking()
                .Include(m => m.Materiale)
                .Include(m => m.TipoMovimentoMateriale)
                .Include(m => m.Dipendente)
                .Include(m => m.Cantiere)
                .Where(m => m.Id == id)
                .Select(m => new MovimentoMaterialeDetailDto
                {
                    Id = m.Id,
                    MaterialeId = m.MaterialeId,
                    Materiale = m.Materiale.Nome,
                    TipoMovimentoMaterialeId = m.TipoMovimentoMaterialeId,
                    TipoMovimentoMateriale = m.TipoMovimentoMateriale.Nome,
                    Quantita = m.Quantita,
                    DataMovimento = m.DataMovimento,
                    DipendenteId = m.DipendenteId,
                    Dipendente = m.Dipendente != null ? m.Dipendente.Cognome + " " + m.Dipendente.Nome : null,
                    CantiereId = m.CantiereId,
                    Cantiere = m.Cantiere != null ? m.Cantiere.Nome : null,
                    RiferimentoTabella = m.RiferimentoTabella,
                    RiferimentoId = m.RiferimentoId,
                    Note = m.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<MovimentoMaterialeDetailDto>> CreateAsync(CreateMovimentoMaterialeDto dto)
        {
            if (dto.Quantita <= 0)
                return ServiceResult<MovimentoMaterialeDetailDto>.Fail("La quantità deve essere maggiore di zero.", 400);

            var validazione = await ValidaRelazioniAsync(
                dto.MaterialeId,
                dto.TipoMovimentoMaterialeId,
                dto.DipendenteId,
                dto.CantiereId);

            if (!validazione.Success)
                return ServiceResult<MovimentoMaterialeDetailDto>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var materiale = await _context.Materialis
                    .FirstAsync(m => m.Id == dto.MaterialeId);

                var direzione = await GetDirezioneMovimentoAsync(dto.TipoMovimentoMaterialeId);
                if (!direzione.Success)
                    return ServiceResult<MovimentoMaterialeDetailDto>.Fail(direzione.Message!, direzione.StatusCode ?? 400);

                var nuovoSaldo = CalcolaNuovoSaldo(materiale.QuantitaAttuale, dto.Quantita, direzione.Data);

                if (nuovoSaldo < 0)
                    return ServiceResult<MovimentoMaterialeDetailDto>.Fail("Il movimento porterebbe il materiale sotto zero.", 400);

                var movimento = new MovimentiMateriale
                {
                    MaterialeId = dto.MaterialeId,
                    TipoMovimentoMaterialeId = dto.TipoMovimentoMaterialeId,
                    Quantita = dto.Quantita,
                    DataMovimento = dto.DataMovimento,
                    DipendenteId = dto.DipendenteId,
                    CantiereId = dto.CantiereId,
                    RiferimentoTabella = Pulisci(dto.RiferimentoTabella),
                    RiferimentoId = dto.RiferimentoId,
                    Note = Pulisci(dto.Note),
                    CreatedAt = DateTime.Now
                };

                materiale.QuantitaAttuale = nuovoSaldo;

                _context.MovimentiMateriales.Add(movimento);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await GetByIdAsync(movimento.Id);
                return ServiceResult<MovimentoMaterialeDetailDto>.Created(result!);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult<MovimentoMaterialeDetailDto>.Fail(
                    $"Errore durante la creazione del movimento: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMovimentoMaterialeDto dto)
        {
            if (dto.Quantita <= 0)
                return ServiceResult<bool>.Fail("La quantità deve essere maggiore di zero.", 400);

            var movimento = await _context.MovimentiMateriales
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimento == null)
                return ServiceResult<bool>.Fail("Movimento materiale non trovato.", 404);

            var validazione = await ValidaRelazioniAsync(
                dto.MaterialeId,
                dto.TipoMovimentoMaterialeId,
                dto.DipendenteId,
                dto.CantiereId);

            if (!validazione.Success)
                return ServiceResult<bool>.Fail(validazione.Message!, validazione.StatusCode ?? 400);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var materialeVecchio = await _context.Materialis
                    .FirstAsync(m => m.Id == movimento.MaterialeId);

                var direzioneVecchia = await GetDirezioneMovimentoAsync(movimento.TipoMovimentoMaterialeId);
                if (!direzioneVecchia.Success)
                    return ServiceResult<bool>.Fail(direzioneVecchia.Message!, direzioneVecchia.StatusCode ?? 400);

                materialeVecchio.QuantitaAttuale = AnnullaSaldo(
                    materialeVecchio.QuantitaAttuale,
                    movimento.Quantita,
                    direzioneVecchia.Data);

                if (materialeVecchio.QuantitaAttuale < 0)
                    return ServiceResult<bool>.Fail("Saldo incoerente durante l'annullamento del movimento precedente.", 400);

                var materialeNuovo = await _context.Materialis
                    .FirstAsync(m => m.Id == dto.MaterialeId);

                var direzioneNuova = await GetDirezioneMovimentoAsync(dto.TipoMovimentoMaterialeId);
                if (!direzioneNuova.Success)
                    return ServiceResult<bool>.Fail(direzioneNuova.Message!, direzioneNuova.StatusCode ?? 400);

                var nuovoSaldo = CalcolaNuovoSaldo(
                    materialeNuovo.QuantitaAttuale,
                    dto.Quantita,
                    direzioneNuova.Data);

                if (nuovoSaldo < 0)
                    return ServiceResult<bool>.Fail("L'aggiornamento porterebbe il materiale sotto zero.", 400);

                materialeNuovo.QuantitaAttuale = nuovoSaldo;

                movimento.MaterialeId = dto.MaterialeId;
                movimento.TipoMovimentoMaterialeId = dto.TipoMovimentoMaterialeId;
                movimento.Quantita = dto.Quantita;
                movimento.DataMovimento = dto.DataMovimento;
                movimento.DipendenteId = dto.DipendenteId;
                movimento.CantiereId = dto.CantiereId;
                movimento.RiferimentoTabella = Pulisci(dto.RiferimentoTabella);
                movimento.RiferimentoId = dto.RiferimentoId;
                movimento.Note = Pulisci(dto.Note);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.Fail(
                    $"Errore durante l'aggiornamento del movimento: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var movimento = await _context.MovimentiMateriales
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimento == null)
                return ServiceResult<bool>.Fail("Movimento materiale non trovato.", 404);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var materiale = await _context.Materialis
                    .FirstAsync(m => m.Id == movimento.MaterialeId);

                var direzione = await GetDirezioneMovimentoAsync(movimento.TipoMovimentoMaterialeId);
                if (!direzione.Success)
                    return ServiceResult<bool>.Fail(direzione.Message!, direzione.StatusCode ?? 400);

                var saldoRipristinato = AnnullaSaldo(
                    materiale.QuantitaAttuale,
                    movimento.Quantita,
                    direzione.Data);

                if (saldoRipristinato < 0)
                    return ServiceResult<bool>.Fail("L'eliminazione del movimento produrrebbe una giacenza negativa incoerente.", 400);

                materiale.QuantitaAttuale = saldoRipristinato;

                _context.MovimentiMateriales.Remove(movimento);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.Fail(
                    $"Errore durante l'eliminazione del movimento: {ex.Message}", 500);
            }
        }

        private async Task<ServiceResult<bool>> ValidaRelazioniAsync(
            int materialeId,
            int tipoMovimentoMaterialeId,
            int? dipendenteId,
            int? cantiereId)
        {
            var materialeEsiste = await _context.Materialis
                .AnyAsync(m => m.Id == materialeId && m.Attivo);

            if (!materialeEsiste)
                return ServiceResult<bool>.Fail("Il materiale selezionato non esiste o non è attivo.", 400);

            var tipoEsiste = await _context.TipiMovimentoMateriales
                .AnyAsync(t => t.Id == tipoMovimentoMaterialeId && t.Attivo);

            if (!tipoEsiste)
                return ServiceResult<bool>.Fail("Il tipo movimento selezionato non esiste o non è attivo.", 400);

            if (dipendenteId.HasValue)
            {
                var dipendenteEsiste = await _context.Dipendentis
                    .AnyAsync(d => d.Id == dipendenteId.Value && d.Attivo);

                if (!dipendenteEsiste)
                    return ServiceResult<bool>.Fail("Il dipendente selezionato non esiste o non è attivo.", 400);
            }

            if (cantiereId.HasValue)
            {
                var cantiereEsiste = await _context.Cantieris
                    .AnyAsync(c => c.Id == cantiereId.Value && c.Attivo);

                if (!cantiereEsiste)
                    return ServiceResult<bool>.Fail("Il cantiere selezionato non esiste o non è attivo.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<int>> GetDirezioneMovimentoAsync(int tipoMovimentoMaterialeId)
        {
            var segno = await _context.TipiMovimentoMateriales
                .Where(t => t.Id == tipoMovimentoMaterialeId && t.Attivo)
                .Select(t => (int?)t.Segno)
                .FirstOrDefaultAsync();

            if (!segno.HasValue)
                return ServiceResult<int>.Fail("Tipo movimento non valido.", 400);

            if (segno.Value != 1 && segno.Value != -1)
                return ServiceResult<int>.Fail("Il segno del tipo movimento deve essere 1 oppure -1.", 400);

            return ServiceResult<int>.Ok(segno.Value);
        }

        private static decimal CalcolaNuovoSaldo(decimal saldoAttuale, decimal quantita, int direzione)
        {
            return direzione > 0
                ? saldoAttuale + quantita
                : saldoAttuale - quantita;
        }

        private static decimal AnnullaSaldo(decimal saldoAttuale, decimal quantita, int direzione)
        {
            return direzione > 0
                ? saldoAttuale - quantita
                : saldoAttuale + quantita;
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class DpiService : IDpiService
    {
        private readonly AppDbContext _context;

        public DpiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DpiListDto>> GetAllAsync()
        {
            return await _context.Dpis
                .Include(d => d.CategoriaDpi)
                .Include(d => d.Fornitore)
                .OrderBy(d => d.Nome)
                .Select(d => new DpiListDto
                {
                    Id = d.Id,
                    Codice = d.Codice,
                    Nome = d.Nome,
                    CategoriaDpiId = d.CategoriaDpiId,
                    CategoriaDpi = d.CategoriaDpi != null ? d.CategoriaDpi.Nome : null,
                    Taglia = d.Taglia,
                    Marca = d.Marca,
                    Modello = d.Modello,
                    Barcode = d.Barcode,
                    FornitoreId = d.FornitoreId,
                    Fornitore = d.Fornitore != null ? d.Fornitore.RagioneSociale : null,
                    DurataGiorni = d.DurataGiorni,
                    HaScadenza = d.HaScadenza,
                    Attivo = d.Attivo
                })
                .ToListAsync();
        }

        public async Task<List<SelectOptionDto>> GetAttiviAsync()
        {
            return await _context.Dpis
                .Where(d => d.Attivo)
                .OrderBy(d => d.Nome)
                .Select(d => new SelectOptionDto
                {
                    Id = d.Id,
                    Nome = d.Nome
                })
                .ToListAsync();
        }

        public async Task<DpiDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Dpis
                .Include(d => d.CategoriaDpi)
                .Include(d => d.Fornitore)
                .Where(d => d.Id == id)
                .Select(d => new DpiDetailDto
                {
                    Id = d.Id,
                    Codice = d.Codice,
                    Nome = d.Nome,
                    CategoriaDpiId = d.CategoriaDpiId,
                    CategoriaDpi = d.CategoriaDpi != null ? d.CategoriaDpi.Nome : null,
                    Descrizione = d.Descrizione,
                    Taglia = d.Taglia,
                    Marca = d.Marca,
                    Modello = d.Modello,
                    Barcode = d.Barcode,
                    FornitoreId = d.FornitoreId,
                    Fornitore = d.Fornitore != null ? d.Fornitore.RagioneSociale : null,
                    DurataGiorni = d.DurataGiorni,
                    HaScadenza = d.HaScadenza,
                    Attivo = d.Attivo,
                    Note = d.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<DpiDetailDto>> CreateAsync(CreateDpiDto dto)
        {
            var validazioneRelazioni = await ValidaRelazioniAsync(dto.CategoriaDpiId, dto.FornitoreId);
            if (!validazioneRelazioni.Success)
                return ServiceResult<DpiDetailDto>.Fail(validazioneRelazioni.Message!, validazioneRelazioni.StatusCode ?? 400);

            var validazioneDuplicati = await ValidaDuplicatiAsync(dto.Codice, dto.Barcode);
            if (!validazioneDuplicati.Success)
                return ServiceResult<DpiDetailDto>.Fail(validazioneDuplicati.Message!, validazioneDuplicati.StatusCode ?? 400);

            var dpi = new Dpi
            {
                Codice = Pulisci(dto.Codice),
                Nome = dto.Nome.Trim(),
                CategoriaDpiId = dto.CategoriaDpiId,
                Descrizione = Pulisci(dto.Descrizione),
                Taglia = Pulisci(dto.Taglia),
                Marca = Pulisci(dto.Marca),
                Modello = Pulisci(dto.Modello),
                Barcode = Pulisci(dto.Barcode),
                FornitoreId = dto.FornitoreId,
                DurataGiorni = dto.DurataGiorni,
                HaScadenza = dto.HaScadenza,
                Attivo = dto.Attivo,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Dpis.Add(dpi);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(dpi.Id);
            return ServiceResult<DpiDetailDto>.Created(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDpiDto dto)
        {
            var dpi = await _context.Dpis.FirstOrDefaultAsync(d => d.Id == id);

            if (dpi == null)
                return ServiceResult<bool>.Fail("DPI non trovato.", 404);

            var validazioneRelazioni = await ValidaRelazioniAsync(dto.CategoriaDpiId, dto.FornitoreId);
            if (!validazioneRelazioni.Success)
                return ServiceResult<bool>.Fail(validazioneRelazioni.Message!, validazioneRelazioni.StatusCode ?? 400);

            var validazioneDuplicati = await ValidaDuplicatiAsync(dto.Codice, dto.Barcode, id);
            if (!validazioneDuplicati.Success)
                return ServiceResult<bool>.Fail(validazioneDuplicati.Message!, validazioneDuplicati.StatusCode ?? 400);

            dpi.Codice = Pulisci(dto.Codice);
            dpi.Nome = dto.Nome.Trim();
            dpi.CategoriaDpiId = dto.CategoriaDpiId;
            dpi.Descrizione = Pulisci(dto.Descrizione);
            dpi.Taglia = Pulisci(dto.Taglia);
            dpi.Marca = Pulisci(dto.Marca);
            dpi.Modello = Pulisci(dto.Modello);
            dpi.Barcode = Pulisci(dto.Barcode);
            dpi.FornitoreId = dto.FornitoreId;
            dpi.DurataGiorni = dto.DurataGiorni;
            dpi.HaScadenza = dto.HaScadenza;
            dpi.Attivo = dto.Attivo;
            dpi.Note = Pulisci(dto.Note);
            dpi.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var dpi = await _context.Dpis.FirstOrDefaultAsync(d => d.Id == id);

            if (dpi == null)
                return ServiceResult<bool>.Fail("DPI non trovato.", 404);

            _context.Dpis.Remove(dpi);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare il DPI perché ha record collegati.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaRelazioniAsync(int? categoriaDpiId, int? fornitoreId)
        {
            if (categoriaDpiId.HasValue)
            {
                var categoriaEsiste = await _context.CategorieDpis
                    .AnyAsync(c => c.Id == categoriaDpiId.Value);

                if (!categoriaEsiste)
                    return ServiceResult<bool>.Fail("La categoria DPI selezionata non esiste.", 400);
            }

            if (fornitoreId.HasValue)
            {
                var fornitoreEsiste = await _context.Fornitoris
                    .AnyAsync(f => f.Id == fornitoreId.Value);

                if (!fornitoreEsiste)
                    return ServiceResult<bool>.Fail("Il fornitore selezionato non esiste.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaDuplicatiAsync(
            string? codice,
            string? barcode,
            int? excludeId = null)
        {
            var codicePulito = Pulisci(codice);
            var barcodePulito = Pulisci(barcode);

            if (!string.IsNullOrWhiteSpace(codicePulito))
            {
                var codiceEsiste = await _context.Dpis.AnyAsync(d =>
                    d.Codice == codicePulito &&
                    (!excludeId.HasValue || d.Id != excludeId.Value));

                if (codiceEsiste)
                    return ServiceResult<bool>.Fail("Esiste già un DPI con questo codice.", 400);
            }

            if (!string.IsNullOrWhiteSpace(barcodePulito))
            {
                var barcodeEsiste = await _context.Dpis.AnyAsync(d =>
                    d.Barcode == barcodePulito &&
                    (!excludeId.HasValue || d.Id != excludeId.Value));

                if (barcodeEsiste)
                    return ServiceResult<bool>.Fail("Esiste già un DPI con questo barcode.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class MaterialiService : IMaterialiService
    {
        private readonly AppDbContext _context;

        public MaterialiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MaterialeListDto>> GetAllAsync()
        {
            return await _context.Materialis
                .Include(m => m.CategoriaMateriale)
                .Include(m => m.Fornitore)
                .OrderBy(m => m.Nome)
                .Select(m => new MaterialeListDto
                {
                    Id = m.Id,
                    Codice = m.Codice,
                    Nome = m.Nome,
                    CategoriaMaterialeId = m.CategoriaMaterialeId,
                    CategoriaMateriale = m.CategoriaMateriale != null ? m.CategoriaMateriale.Nome : null,
                    UnitaMisura = m.UnitaMisura,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima,
                    SottoScorta = m.ScortaMinima.HasValue && m.QuantitaAttuale <= m.ScortaMinima.Value,
                    Barcode = m.Barcode,
                    FornitoreId = m.FornitoreId,
                    Fornitore = m.Fornitore != null ? m.Fornitore.RagioneSociale : null,
                    Attivo = m.Attivo
                })
                .ToListAsync();
        }

        public async Task<List<MaterialeListDto>> GetSottoScortaAsync()
        {
            return await _context.Materialis
                .Include(m => m.CategoriaMateriale)
                .Include(m => m.Fornitore)
                .Where(m => m.ScortaMinima.HasValue && m.QuantitaAttuale <= m.ScortaMinima.Value)
                .OrderBy(m => m.Nome)
                .Select(m => new MaterialeListDto
                {
                    Id = m.Id,
                    Codice = m.Codice,
                    Nome = m.Nome,
                    CategoriaMaterialeId = m.CategoriaMaterialeId,
                    CategoriaMateriale = m.CategoriaMateriale != null ? m.CategoriaMateriale.Nome : null,
                    UnitaMisura = m.UnitaMisura,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima,
                    SottoScorta = true,
                    Barcode = m.Barcode,
                    FornitoreId = m.FornitoreId,
                    Fornitore = m.Fornitore != null ? m.Fornitore.RagioneSociale : null,
                    Attivo = m.Attivo
                })
                .ToListAsync();
        }

        public async Task<List<SelectOptionDto>> GetAttiviAsync()
        {
            return await _context.Materialis
                .Where(m => m.Attivo)
                .OrderBy(m => m.Nome)
                .Select(m => new SelectOptionDto
                {
                    Id = m.Id,
                    Nome = m.Nome
                })
                .ToListAsync();
        }

        public async Task<MaterialeDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Materialis
                .Include(m => m.CategoriaMateriale)
                .Include(m => m.Fornitore)
                .Where(m => m.Id == id)
                .Select(m => new MaterialeDetailDto
                {
                    Id = m.Id,
                    Codice = m.Codice,
                    Nome = m.Nome,
                    CategoriaMaterialeId = m.CategoriaMaterialeId,
                    CategoriaMateriale = m.CategoriaMateriale != null ? m.CategoriaMateriale.Nome : null,
                    Descrizione = m.Descrizione,
                    UnitaMisura = m.UnitaMisura,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima,
                    SottoScorta = m.ScortaMinima.HasValue && m.QuantitaAttuale <= m.ScortaMinima.Value,
                    Barcode = m.Barcode,
                    FornitoreId = m.FornitoreId,
                    Fornitore = m.Fornitore != null ? m.Fornitore.RagioneSociale : null,
                    Attivo = m.Attivo,
                    Note = m.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<MaterialeDetailDto>> CreateAsync(CreateMaterialeDto dto)
        {
            var validazioneRelazioni = await ValidaRelazioniAsync(dto.CategoriaMaterialeId, dto.FornitoreId);
            if (!validazioneRelazioni.Success)
                return ServiceResult<MaterialeDetailDto>.Fail(validazioneRelazioni.Message!, validazioneRelazioni.StatusCode ?? 400);

            var validazioneDuplicati = await ValidaDuplicatiAsync(dto.Codice, dto.Barcode);
            if (!validazioneDuplicati.Success)
                return ServiceResult<MaterialeDetailDto>.Fail(validazioneDuplicati.Message!, validazioneDuplicati.StatusCode ?? 400);

            var materiale = new Materiali
            {
                Codice = Pulisci(dto.Codice),
                Nome = dto.Nome.Trim(),
                CategoriaMaterialeId = dto.CategoriaMaterialeId,
                Descrizione = Pulisci(dto.Descrizione),
                UnitaMisura = Pulisci(dto.UnitaMisura),
                QuantitaAttuale = dto.QuantitaAttuale,
                ScortaMinima = dto.ScortaMinima,
                Barcode = Pulisci(dto.Barcode),
                FornitoreId = dto.FornitoreId,
                Attivo = dto.Attivo,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Materialis.Add(materiale);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(materiale.Id);
            return ServiceResult<MaterialeDetailDto>.Created(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMaterialeDto dto)
        {
            var materiale = await _context.Materialis.FirstOrDefaultAsync(m => m.Id == id);

            if (materiale == null)
                return ServiceResult<bool>.Fail("Materiale non trovato.", 404);

            var validazioneRelazioni = await ValidaRelazioniAsync(dto.CategoriaMaterialeId, dto.FornitoreId);
            if (!validazioneRelazioni.Success)
                return ServiceResult<bool>.Fail(validazioneRelazioni.Message!, validazioneRelazioni.StatusCode ?? 400);

            var validazioneDuplicati = await ValidaDuplicatiAsync(dto.Codice, dto.Barcode, id);
            if (!validazioneDuplicati.Success)
                return ServiceResult<bool>.Fail(validazioneDuplicati.Message!, validazioneDuplicati.StatusCode ?? 400);

            materiale.Codice = Pulisci(dto.Codice);
            materiale.Nome = dto.Nome.Trim();
            materiale.CategoriaMaterialeId = dto.CategoriaMaterialeId;
            materiale.Descrizione = Pulisci(dto.Descrizione);
            materiale.UnitaMisura = Pulisci(dto.UnitaMisura);
            materiale.QuantitaAttuale = dto.QuantitaAttuale;
            materiale.ScortaMinima = dto.ScortaMinima;
            materiale.Barcode = Pulisci(dto.Barcode);
            materiale.FornitoreId = dto.FornitoreId;
            materiale.Attivo = dto.Attivo;
            materiale.Note = Pulisci(dto.Note);
            materiale.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var materiale = await _context.Materialis.FirstOrDefaultAsync(m => m.Id == id);

            if (materiale == null)
                return ServiceResult<bool>.Fail("Materiale non trovato.", 404);

            _context.Materialis.Remove(materiale);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare il materiale perché ha record collegati.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaRelazioniAsync(int? categoriaMaterialeId, int? fornitoreId)
        {
            if (categoriaMaterialeId.HasValue)
            {
                var categoriaEsiste = await _context.CategorieMateriales
                    .AnyAsync(c => c.Id == categoriaMaterialeId.Value);

                if (!categoriaEsiste)
                    return ServiceResult<bool>.Fail("La categoria materiale selezionata non esiste.", 400);
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
                var codiceEsiste = await _context.Materialis.AnyAsync(m =>
                    m.Codice == codicePulito &&
                    (!excludeId.HasValue || m.Id != excludeId.Value));

                if (codiceEsiste)
                    return ServiceResult<bool>.Fail("Esiste già un materiale con questo codice.", 400);
            }

            if (!string.IsNullOrWhiteSpace(barcodePulito))
            {
                var barcodeEsiste = await _context.Materialis.AnyAsync(m =>
                    m.Barcode == barcodePulito &&
                    (!excludeId.HasValue || m.Id != excludeId.Value));

                if (barcodeEsiste)
                    return ServiceResult<bool>.Fail("Esiste già un materiale con questo barcode.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
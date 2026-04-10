using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class MagazzinoService : IMagazzinoService
    {
        private readonly AppDbContext _context;
        private readonly IMovimentiMaterialeService _movimentiService;

        public MagazzinoService(
            AppDbContext context,
            IMovimentiMaterialeService movimentiService)
        {
            _context = context;
            _movimentiService = movimentiService;
        }

        public async Task<ServiceResult<bool>> PrelevaAsync(PrelievoMaterialeDto dto)
        {
            if (dto.Quantita <= 0)
                return ServiceResult<bool>.Fail("La quantità deve essere maggiore di zero.", 400);

            var materialeEsiste = await _context.Materialis
                .AnyAsync(m => m.Id == dto.MaterialeId && m.Attivo);

            if (!materialeEsiste)
                return ServiceResult<bool>.Fail("Materiale non trovato o non attivo.", 400);

            var tipoScarico = await _context.TipiMovimentoMateriales
                .Where(t => t.Attivo && t.Segno == -1)
                .OrderBy(t => t.Id)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            if (tipoScarico == 0)
                return ServiceResult<bool>.Fail("Tipo movimento di scarico non configurato.", 400);

            var movimento = new CreateMovimentoMaterialeDto
            {
                MaterialeId = dto.MaterialeId,
                TipoMovimentoMaterialeId = tipoScarico,
                Quantita = dto.Quantita,
                DataMovimento = DateTime.Now,
                DipendenteId = dto.DipendenteId,
                CantiereId = dto.CantiereId,
                Note = Pulisci(dto.Note)
            };

            var result = await _movimentiService.CreateAsync(movimento);

            if (!result.Success)
                return ServiceResult<bool>.Fail(result.Message!, result.StatusCode ?? 400);

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> RifornisciAsync(PrelievoMaterialeDto dto)
        {
            if (dto.Quantita <= 0)
                return ServiceResult<bool>.Fail("La quantità deve essere maggiore di zero.", 400);

            var materialeEsiste = await _context.Materialis
                .AnyAsync(m => m.Id == dto.MaterialeId && m.Attivo);

            if (!materialeEsiste)
                return ServiceResult<bool>.Fail("Materiale non trovato o non attivo.", 400);

            var tipoCarico = await _context.TipiMovimentoMateriales
                .Where(t => t.Attivo && t.Segno == 1)
                .OrderBy(t => t.Id)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            if (tipoCarico == 0)
                return ServiceResult<bool>.Fail("Tipo movimento di carico non configurato.", 400);

            var movimento = new CreateMovimentoMaterialeDto
            {
                MaterialeId = dto.MaterialeId,
                TipoMovimentoMaterialeId = tipoCarico,
                Quantita = dto.Quantita,
                DataMovimento = DateTime.Now,
                DipendenteId = dto.DipendenteId,
                CantiereId = dto.CantiereId,
                Note = Pulisci(dto.Note)
            };

            var result = await _movimentiService.CreateAsync(movimento);

            if (!result.Success)
                return ServiceResult<bool>.Fail(result.Message!, result.StatusCode ?? 400);

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<MaterialeScannerDto>> GetByEanAsync(string ean)
        {
            if (string.IsNullOrWhiteSpace(ean))
                return ServiceResult<MaterialeScannerDto>.Fail("Barcode obbligatorio.", 400);

            var barcode = ean.Trim();

            var materiale = await _context.Materialis
                .AsNoTracking()
                .Include(m => m.CategoriaMateriale)
                .Where(m => m.Attivo && m.Barcode == barcode)
                .Select(m => new MaterialeScannerDto
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    CodiceInterno = m.Codice,
                    Ean = m.Barcode,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima ?? 0,
                    SottoScorta = m.QuantitaAttuale <= (m.ScortaMinima ?? 0),
                    Categoria = m.CategoriaMateriale != null ? m.CategoriaMateriale.Nome : null,
                    UnitaMisura = m.UnitaMisura
                })
                .FirstOrDefaultAsync();

            if (materiale == null)
                return ServiceResult<MaterialeScannerDto>.Fail("Materiale non trovato.", 404);

            return ServiceResult<MaterialeScannerDto>.Ok(materiale);
        }

        public async Task<List<MaterialeSottoScortaDto>> GetMaterialiSottoScortaAsync()
        {
            return await _context.Materialis
                .AsNoTracking()
                .Include(m => m.CategoriaMateriale)
                .Where(m => m.Attivo && m.QuantitaAttuale <= (m.ScortaMinima ?? 0))
                .OrderBy(m => m.QuantitaAttuale)
                .ThenBy(m => m.Nome)
                .Select(m => new MaterialeSottoScortaDto
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    CodiceInterno = m.Codice,
                    Ean = m.Barcode,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima ?? 0,
                    Differenza = m.QuantitaAttuale - (m.ScortaMinima ?? 0),
                    Categoria = m.CategoriaMateriale != null ? m.CategoriaMateriale.Nome : null
                })
                .ToListAsync();
        }

        public async Task<DashboardMagazzinoDto> GetDashboardAsync()
        {
            var query = _context.Materialis.AsNoTracking();

            return new DashboardMagazzinoDto
            {
                TotaleMateriali = await query.CountAsync(),
                MaterialiAttivi = await query.CountAsync(m => m.Attivo),
                MaterialiSottoScorta = await query.CountAsync(m => m.Attivo && m.QuantitaAttuale <= (m.ScortaMinima ?? 0)),
                MaterialiEsauriti = await query.CountAsync(m => m.Attivo && m.QuantitaAttuale <= 0),
                QuantitaTotale = await query
                    .Where(m => m.Attivo)
                    .SumAsync(m => (decimal?)m.QuantitaAttuale) ?? 0
            };
        }
        public async Task<List<StoricoMovimentoMaterialeDto>> GetStoricoMovimentiMaterialeAsync(int materialeId)
        {
            return await _context.MovimentiMateriales
                .AsNoTracking()
                .Include(m => m.TipoMovimentoMateriale)
                .Include(m => m.Dipendente)
                .Include(m => m.Cantiere)
                .Where(m => m.MaterialeId == materialeId)
                .OrderByDescending(m => m.DataMovimento)
                .ThenByDescending(m => m.Id)
                .Select(m => new StoricoMovimentoMaterialeDto
                {
                    Id = m.Id,
                    DataMovimento = m.DataMovimento,
                    TipoMovimento = m.TipoMovimentoMateriale.Nome,
                    Segno = m.TipoMovimentoMateriale.Segno,
                    Quantita = m.Quantita,
                    Dipendente = m.Dipendente != null ? m.Dipendente.Cognome + " " + m.Dipendente.Nome : null,
                    Cantiere = m.Cantiere != null ? m.Cantiere.Nome : null,
                    RiferimentoTabella = m.RiferimentoTabella,
                    RiferimentoId = m.RiferimentoId,
                    Note = m.Note
                })
                .ToListAsync();
        }
        public async Task<List<RicercaMaterialeDto>> RicercaMaterialiAsync(string? testo)
        {
            var query = _context.Materialis
                .AsNoTracking()
                .Include(m => m.CategoriaMateriale)
                .Where(m => m.Attivo);

            if (!string.IsNullOrWhiteSpace(testo))
            {
                var filtro = testo.Trim();

                query = query.Where(m =>
                    m.Nome.Contains(filtro) ||
                    (m.Codice != null && m.Codice.Contains(filtro)) ||
                    (m.Barcode != null && m.Barcode.Contains(filtro)));
            }

            return await query
                .OrderBy(m => m.Nome)
                .Select(m => new RicercaMaterialeDto
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Codice = m.Codice,
                    Barcode = m.Barcode,
                    Categoria = m.CategoriaMateriale != null ? m.CategoriaMateriale.Nome : null,
                    UnitaMisura = m.UnitaMisura,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima ?? 0,
                    SottoScorta = m.QuantitaAttuale <= (m.ScortaMinima ?? 0),
                    Attivo = m.Attivo
                })
                .ToListAsync();
        }
        public async Task<ServiceResult<DettaglioMaterialeDto>> GetMaterialeByIdAsync(int id)
        {
            var materiale = await _context.Materialis
                .AsNoTracking()
                .Include(m => m.CategoriaMateriale)
                .Include(m => m.Fornitore)
                .Where(m => m.Id == id)
                .Select(m => new DettaglioMaterialeDto
                {
                    Id = m.Id,
                    Codice = m.Codice,
                    Nome = m.Nome,
                    CategoriaMaterialeId = m.CategoriaMaterialeId,
                    CategoriaMateriale = m.CategoriaMateriale != null ? m.CategoriaMateriale.Nome : null,
                    Descrizione = m.Descrizione,
                    UnitaMisura = m.UnitaMisura,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima ?? 0,
                    SottoScorta = m.QuantitaAttuale <= (m.ScortaMinima ?? 0),
                    Barcode = m.Barcode,
                    FornitoreId = m.FornitoreId,
                    Fornitore = m.Fornitore != null ? m.Fornitore.RagioneSociale : null,
                    Attivo = m.Attivo,
                    Note = m.Note,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (materiale == null)
                return ServiceResult<DettaglioMaterialeDto>.Fail("Materiale non trovato.", 404);

            return ServiceResult<DettaglioMaterialeDto>.Ok(materiale);
        }
        public async Task<List<UltimoMovimentoMagazzinoDto>> GetUltimiMovimentiAsync(int top = 10)
        {
            if (top <= 0)
                top = 10;

            if (top > 100)
                top = 100;

            return await _context.MovimentiMateriales
                .AsNoTracking()
                .Include(m => m.Materiale)
                .Include(m => m.TipoMovimentoMateriale)
                .Include(m => m.Dipendente)
                .Include(m => m.Cantiere)
                .OrderByDescending(m => m.DataMovimento)
                .ThenByDescending(m => m.Id)
                .Take(top)
                .Select(m => new UltimoMovimentoMagazzinoDto
                {
                    Id = m.Id,
                    DataMovimento = m.DataMovimento,
                    MaterialeId = m.MaterialeId,
                    Materiale = m.Materiale.Nome,
                    TipoMovimento = m.TipoMovimentoMateriale.Nome,
                    Segno = m.TipoMovimentoMateriale.Segno,
                    Quantita = m.Quantita,
                    Dipendente = m.Dipendente != null ? m.Dipendente.Cognome + " " + m.Dipendente.Nome : null,
                    Cantiere = m.Cantiere != null ? m.Cantiere.Nome : null,
                    Note = m.Note
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<DisponibilitaMaterialeDto>> GetDisponibilitaMaterialeAsync(int id)
        {
            var materiale = await _context.Materialis
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new DisponibilitaMaterialeDto
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Codice = m.Codice,
                    Barcode = m.Barcode,
                    QuantitaAttuale = m.QuantitaAttuale,
                    ScortaMinima = m.ScortaMinima ?? 0,
                    SottoScorta = m.QuantitaAttuale <= (m.ScortaMinima ?? 0),
                    Disponibile = m.Attivo && m.QuantitaAttuale > 0,
                    UnitaMisura = m.UnitaMisura
                })
                .FirstOrDefaultAsync();

            if (materiale == null)
                return ServiceResult<DisponibilitaMaterialeDto>.Fail("Materiale non trovato.", 404);

            return ServiceResult<DisponibilitaMaterialeDto>.Ok(materiale);
        }
        public async Task<List<MaterialeSelectDto>> GetMaterialiSelectAsync()
        {
            return await _context.Materialis
                .AsNoTracking()
                .Where(m => m.Attivo)
                .OrderBy(m => m.Nome)
                .Select(m => new MaterialeSelectDto
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Codice = m.Codice,
                    Barcode = m.Barcode,
                    QuantitaAttuale = m.QuantitaAttuale,
                    UnitaMisura = m.UnitaMisura,
                    Label = string.IsNullOrWhiteSpace(m.Codice)
                        ? m.Nome
                        : m.Codice + " - " + m.Nome
                })
                .ToListAsync();
        }

        public async Task<List<DipendenteSelectDto>> GetDipendentiSelectAsync()
        {
            return await _context.Dipendentis
                .AsNoTracking()
                .Where(d => d.Attivo)
                .OrderBy(d => d.Cognome)
                .ThenBy(d => d.Nome)
                .Select(d => new DipendenteSelectDto
                {
                    Id = d.Id,
                    NomeCompleto = d.Cognome + " " + d.Nome,
                    Label = d.Cognome + " " + d.Nome
                })
                .ToListAsync();
        }

        public async Task<List<CantiereSelectDto>> GetCantieriSelectAsync()
        {
            return await _context.Cantieris
                .AsNoTracking()
                .Where(c => c.Attivo)
                .OrderBy(c => c.Nome)
                .Select(c => new CantiereSelectDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Label = c.Nome
                })
                .ToListAsync();
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
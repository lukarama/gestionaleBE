using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class CantieriService : ICantieriService
    {
        private readonly AppDbContext _context;

        public CantieriService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CantiereListDto>> GetAllAsync()
        {
            return await _context.Cantieris
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .Select(c => new CantiereListDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Indirizzo = c.Indirizzo,
                    ResponsabileCantiere = c.ResponsabileCantiere,
                    Committente = c.Committente,
                    DataInizioLavori = c.DataInizioLavori,
                    DataPrevistaFineLavori = c.DataPrevistaFineLavori,
                    Attivo = c.Attivo
                })
                .ToListAsync();
        }

        public async Task<List<SelectOptionDto>> GetAttiviAsync()
        {
            return await _context.Cantieris
                .AsNoTracking()
                .Where(c => c.Attivo)
                .OrderBy(c => c.Nome)
                .Select(c => new SelectOptionDto
                {
                    Id = c.Id,
                    Nome = c.Nome
                })
                .ToListAsync();
        }

        public async Task<CantiereDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Cantieris
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CantiereDetailDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Indirizzo = c.Indirizzo,
                    ResponsabileCantiere = c.ResponsabileCantiere,
                    DirezioneLavori = c.DirezioneLavori,
                    Committente = c.Committente,
                    Appaltatore = c.Appaltatore,
                    DataInizioLavori = c.DataInizioLavori,
                    DataPrevistaFineLavori = c.DataPrevistaFineLavori,
                    Attivo = c.Attivo,
                    Note = c.Note,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CantiereSchedaDto?> GetSchedaAsync(int id)
        {
            var cantiere = await GetByIdAsync(id);
            if (cantiere == null)
            {
                return null;
            }

            var documenti = await _context.DocumentiCantieris
                .AsNoTracking()
                .Include(d => d.Cantiere)
                .Where(d => d.CantiereId == id)
                .OrderByDescending(d => d.DataDocumento)
                .ThenByDescending(d => d.Id)
                .Select(d => new DocumentoCantiereListDto
                {
                    Id = d.Id,
                    CantiereId = d.CantiereId,
                    Cantiere = d.Cantiere.Nome,
                    NomeFile = d.NomeFile,
                    PercorsoFile = d.PercorsoFile,
                    Estensione = d.Estensione,
                    ContentType = d.ContentType,
                    DataDocumento = d.DataDocumento
                })
                .ToListAsync();

            var movimenti = await _context.MovimentiMateriales
                .AsNoTracking()
                .Include(m => m.Materiale)
                .Include(m => m.TipoMovimentoMateriale)
                .Include(m => m.Dipendente)
                .Include(m => m.Cantiere)
                .Where(m => m.CantiereId == id)
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

            return new CantiereSchedaDto
            {
                Cantiere = cantiere,
                Documenti = documenti,
                MovimentiMateriale = movimenti
            };
        }

        public async Task<ServiceResult<CantiereDetailDto>> CreateAsync(CreateCantiereDto dto)
        {
            var validazioneDate = ValidaDate(dto.DataInizioLavori, dto.DataPrevistaFineLavori);
            if (!validazioneDate.Success)
            {
                return ServiceResult<CantiereDetailDto>.Fail(validazioneDate.Message!, validazioneDate.StatusCode ?? 400);
            }

            var nomePulito = Pulisci(dto.Nome);
            if (string.IsNullOrWhiteSpace(nomePulito))
            {
                return ServiceResult<CantiereDetailDto>.Fail("Il nome del cantiere è obbligatorio.", 400);
            }

            var nomeEsiste = await _context.Cantieris
                .AnyAsync(c => c.Nome == nomePulito);

            if (nomeEsiste)
            {
                return ServiceResult<CantiereDetailDto>.Fail("Esiste già un cantiere con questo nome.", 400);
            }

            var cantiere = new Cantieri
            {
                Nome = nomePulito,
                Indirizzo = Pulisci(dto.Indirizzo),
                ResponsabileCantiere = Pulisci(dto.ResponsabileCantiere),
                DirezioneLavori = Pulisci(dto.DirezioneLavori),
                Committente = Pulisci(dto.Committente),
                Appaltatore = Pulisci(dto.Appaltatore),
                DataInizioLavori = dto.DataInizioLavori,
                DataPrevistaFineLavori = dto.DataPrevistaFineLavori,
                Attivo = dto.Attivo,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Cantieris.Add(cantiere);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(cantiere.Id);
            return ServiceResult<CantiereDetailDto>.Created(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCantiereDto dto)
        {
            var cantiere = await _context.Cantieris
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cantiere == null)
            {
                return ServiceResult<bool>.Fail("Cantiere non trovato.", 404);
            }

            var validazioneDate = ValidaDate(dto.DataInizioLavori, dto.DataPrevistaFineLavori);
            if (!validazioneDate.Success)
            {
                return ServiceResult<bool>.Fail(validazioneDate.Message!, validazioneDate.StatusCode ?? 400);
            }

            var nomePulito = Pulisci(dto.Nome);
            if (string.IsNullOrWhiteSpace(nomePulito))
            {
                return ServiceResult<bool>.Fail("Il nome del cantiere è obbligatorio.", 400);
            }

            var nomeEsiste = await _context.Cantieris
                .AnyAsync(c => c.Id != id && c.Nome == nomePulito);

            if (nomeEsiste)
            {
                return ServiceResult<bool>.Fail("Esiste già un altro cantiere con questo nome.", 400);
            }

            cantiere.Nome = nomePulito;
            cantiere.Indirizzo = Pulisci(dto.Indirizzo);
            cantiere.ResponsabileCantiere = Pulisci(dto.ResponsabileCantiere);
            cantiere.DirezioneLavori = Pulisci(dto.DirezioneLavori);
            cantiere.Committente = Pulisci(dto.Committente);
            cantiere.Appaltatore = Pulisci(dto.Appaltatore);
            cantiere.DataInizioLavori = dto.DataInizioLavori;
            cantiere.DataPrevistaFineLavori = dto.DataPrevistaFineLavori;
            cantiere.Attivo = dto.Attivo;
            cantiere.Note = Pulisci(dto.Note);
            cantiere.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var cantiere = await _context.Cantieris
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cantiere == null)
            {
                return ServiceResult<bool>.Fail("Cantiere non trovato.", 404);
            }

            var usato = await _context.AssegnazioniDpis.AnyAsync(a => a.CantiereId == id)
                      || await _context.AssegnazioniMaterialis.AnyAsync(a => a.CantiereId == id)
                      || await _context.AssegnazioniMezzis.AnyAsync(a => a.CantiereId == id)
                      || await _context.MovimentiMateriales.AnyAsync(m => m.CantiereId == id)
                      || await _context.DocumentiCantieris.AnyAsync(d => d.CantiereId == id);

            if (usato)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare il cantiere perché è collegato ad altri record.", 400);
            }

            _context.Cantieris.Remove(cantiere);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private static ServiceResult<bool> ValidaDate(DateOnly? dataInizioLavori, DateOnly? dataPrevistaFineLavori)
        {
            if (dataInizioLavori.HasValue &&
                dataPrevistaFineLavori.HasValue &&
                dataPrevistaFineLavori.Value < dataInizioLavori.Value)
            {
                return ServiceResult<bool>.Fail("La data prevista di fine lavori non può essere precedente alla data di inizio lavori.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}

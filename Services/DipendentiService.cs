using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class DipendentiService : IDipendentiService
    {
        private readonly AppDbContext _context;
        private readonly IDocumentiDipendentiStorageService _documentiStorageService;
        private readonly ILogger<DipendentiService> _logger;

        public DipendentiService(
            AppDbContext context,
            IDocumentiDipendentiStorageService documentiStorageService,
            ILogger<DipendentiService> logger)
        {
            _context = context;
            _documentiStorageService = documentiStorageService;
            _logger = logger;
        }

        public async Task<List<DipendenteListDto>> GetAllAsync()
        {
            return await _context.Dipendentis
                .Include(d => d.Mansione)
                .OrderBy(d => d.Cognome)
                .ThenBy(d => d.Nome)
                .Select(d => new DipendenteListDto
                {
                    Id = d.Id,
                    Matricola = d.Matricola,
                    Nome = d.Nome,
                    Cognome = d.Cognome,
                    Telefono = d.Telefono,
                    Email = d.Email,
                    Attivo = d.Attivo,
                    Mansione = d.Mansione != null ? d.Mansione.Nome : null
                })
                .ToListAsync();
        }

        public async Task<DipendenteDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Dipendentis
                .Include(d => d.Mansione)
                .Where(d => d.Id == id)
                .Select(d => new DipendenteDetailDto
                {
                    Id = d.Id,
                    Matricola = d.Matricola,
                    Nome = d.Nome,
                    Cognome = d.Cognome,
                    CodiceFiscale = d.CodiceFiscale,
                    DataNascita = d.DataNascita,
                    LuogoNascita = d.LuogoNascita,
                    Telefono = d.Telefono,
                    Email = d.Email,
                    Indirizzo = d.Indirizzo,
                    Citta = d.Citta,
                    Provincia = d.Provincia,
                    Cap = d.Cap,
                    DataAssunzione = d.DataAssunzione,
                    DataCessazione = d.DataCessazione,
                    HaPatente = d.HaPatente,
                    CategoriaPatente = d.CategoriaPatente,
                    MansioneId = d.MansioneId,
                    Mansione = d.Mansione != null ? d.Mansione.Nome : null,
                    Attivo = d.Attivo,
                    Note = d.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DipendenteSchedaDto?> GetSchedaAsync(int id)
        {
            var dipendente = await GetByIdAsync(id);
            if (dipendente == null)
            {
                return null;
            }

            var documenti = await _context.DocumentiDipendentis
                .AsNoTracking()
                .Include(d => d.Dipendente)
                .Include(d => d.TipoDocumento)
                .Where(d => d.DipendenteId == id)
                .OrderByDescending(d => d.DataDocumento)
                .ThenByDescending(d => d.Id)
                .Select(d => new DocumentoDipendenteListDto
                {
                    Id = d.Id,
                    DipendenteId = d.DipendenteId,
                    Dipendente = d.Dipendente.Cognome + " " + d.Dipendente.Nome,
                    TipoDocumentoId = d.TipoDocumentoId,
                    TipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nome : null,
                    NomeFile = d.NomeFile,
                    PercorsoFile = d.PercorsoFile,
                    Estensione = d.Estensione,
                    ContentType = d.ContentType,
                    DataDocumento = d.DataDocumento,
                    DataScadenza = d.DataScadenza
                })
                .ToListAsync();

            var visiteMediche = await _context.VisiteMediches
                .AsNoTracking()
                .Include(v => v.Dipendente)
                .Include(v => v.TipoVisitaMedica)
                .Include(v => v.EsitoVisitaMedica)
                .Where(v => v.DipendenteId == id)
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

            var dpiAssegnati = await _context.AssegnazioniDpis
                .AsNoTracking()
                .Include(a => a.Dipendente)
                .Include(a => a.Dpi)
                .Include(a => a.Cantiere)
                .Include(a => a.StatoAssegnazione)
                .Where(a => a.DipendenteId == id)
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

            return new DipendenteSchedaDto
            {
                Dipendente = dipendente,
                Documenti = documenti,
                VisiteMediche = visiteMediche,
                DpiAssegnati = dpiAssegnati
            };
        }

        public async Task<ServiceResult<DipendenteDetailDto>> CreateAsync(CreateDipendenteDto dto)
        {
            var validazione = await ValidaMansioneAsync(dto.MansioneId);
            if (!validazione.Success)
            {
                return ServiceResult<DipendenteDetailDto>.Fail(validazione.Message!);
            }

            var dipendente = new Dipendenti
            {
                Matricola = Pulisci(dto.Matricola),
                Nome = dto.Nome.Trim(),
                Cognome = dto.Cognome.Trim(),
                CodiceFiscale = Pulisci(dto.CodiceFiscale),
                DataNascita = dto.DataNascita,
                LuogoNascita = Pulisci(dto.LuogoNascita),
                Telefono = Pulisci(dto.Telefono),
                Email = Pulisci(dto.Email),
                Indirizzo = Pulisci(dto.Indirizzo),
                Citta = Pulisci(dto.Citta),
                Provincia = Pulisci(dto.Provincia),
                Cap = Pulisci(dto.Cap),
                DataAssunzione = dto.DataAssunzione,
                DataCessazione = dto.DataCessazione,
                HaPatente = dto.HaPatente,
                CategoriaPatente = dto.HaPatente ? Pulisci(dto.CategoriaPatente) : null,
                MansioneId = dto.MansioneId,
                Attivo = dto.Attivo,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Dipendentis.Add(dipendente);
            await _context.SaveChangesAsync();

            _context.CartelleDocumentiDipendentis.AddRange(CreateCartelleDocumentiStandard(dipendente.Id));
            await _context.SaveChangesAsync();

            try
            {
                _documentiStorageService.EnsureDipendenteFolder(dipendente);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                _logger.LogWarning(
                    ex,
                    "Impossibile creare la cartella documenti per il dipendente {DipendenteId}.",
                    dipendente.Id);
            }

            var result = await GetByIdAsync(dipendente.Id);

            return ServiceResult<DipendenteDetailDto>.Ok(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDipendenteDto dto)
        {
            var dipendente = await _context.Dipendentis
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dipendente == null)
            {
                return ServiceResult<bool>.Fail("Dipendente non trovato.");
            }

            var validazione = await ValidaMansioneAsync(dto.MansioneId);
            if (!validazione.Success)
            {
                return ServiceResult<bool>.Fail(validazione.Message!);
            }

            dipendente.Matricola = Pulisci(dto.Matricola);
            dipendente.Nome = dto.Nome.Trim();
            dipendente.Cognome = dto.Cognome.Trim();
            dipendente.CodiceFiscale = Pulisci(dto.CodiceFiscale);
            dipendente.DataNascita = dto.DataNascita;
            dipendente.LuogoNascita = Pulisci(dto.LuogoNascita);
            dipendente.Telefono = Pulisci(dto.Telefono);
            dipendente.Email = Pulisci(dto.Email);
            dipendente.Indirizzo = Pulisci(dto.Indirizzo);
            dipendente.Citta = Pulisci(dto.Citta);
            dipendente.Provincia = Pulisci(dto.Provincia);
            dipendente.Cap = Pulisci(dto.Cap);
            dipendente.DataAssunzione = dto.DataAssunzione;
            dipendente.DataCessazione = dto.DataCessazione;
            dipendente.HaPatente = dto.HaPatente;
            dipendente.CategoriaPatente = dto.HaPatente ? Pulisci(dto.CategoriaPatente) : null;
            dipendente.MansioneId = dto.MansioneId;
            dipendente.Attivo = dto.Attivo;
            dipendente.Note = Pulisci(dto.Note);
            dipendente.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var dipendente = await _context.Dipendentis
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dipendente == null)
            {
                return ServiceResult<bool>.Fail("Dipendente non trovato.");
            }

            _context.Dipendentis.Remove(dipendente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare il dipendente perché ha record collegati.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaMansioneAsync(int? mansioneId)
        {
            if (!mansioneId.HasValue)
            {
                return ServiceResult<bool>.Ok(true);
            }

            var mansioneEsiste = await _context.Mansionis
                .AnyAsync(m => m.Id == mansioneId.Value && m.Attivo);

            if (!mansioneEsiste)
            {
                return ServiceResult<bool>.Fail("La mansione selezionata non esiste o non è attiva.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }

        private static IEnumerable<CartelleDocumentiDipendenti> CreateCartelleDocumentiStandard(int dipendenteId)
        {
            var now = DateTime.Now;
            var nomi = new[] { "Cedolini", "Contratti", "CU", "Documenti personali", "Altro" };

            return nomi.Select(nome => new CartelleDocumentiDipendenti
            {
                DipendenteId = dipendenteId,
                Nome = nome,
                CreatedAt = now
            });
        }
    }
}

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

        public DipendentiService(AppDbContext context)
        {
            _context = context;
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
                    MansioneId = d.MansioneId,
                    Mansione = d.Mansione != null ? d.Mansione.Nome : null,
                    Attivo = d.Attivo,
                    Note = d.Note
                })
                .FirstOrDefaultAsync();
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
                MansioneId = dto.MansioneId,
                Attivo = dto.Attivo,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Dipendentis.Add(dipendente);
            await _context.SaveChangesAsync();

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
    }
}
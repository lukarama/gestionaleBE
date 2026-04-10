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
                .OrderBy(c => c.Nome)
                .Select(c => new CantiereListDto
                {
                    Id = c.Id,
                    Codice = c.Codice,
                    Nome = c.Nome,
                    Cliente = c.Cliente,
                    Citta = c.Citta,
                    Provincia = c.Provincia,
                    DataInizio = c.DataInizio,
                    DataFine = c.DataFine,
                    Attivo = c.Attivo
                })
                .ToListAsync();
        }

        public async Task<List<SelectOptionDto>> GetAttiviAsync()
        {
            return await _context.Cantieris
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
                .Where(c => c.Id == id)
                .Select(c => new CantiereDetailDto
                {
                    Id = c.Id,
                    Codice = c.Codice,
                    Nome = c.Nome,
                    Cliente = c.Cliente,
                    Indirizzo = c.Indirizzo,
                    Citta = c.Citta,
                    Provincia = c.Provincia,
                    Cap = c.Cap,
                    DataInizio = c.DataInizio,
                    DataFine = c.DataFine,
                    Attivo = c.Attivo,
                    Note = c.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<CantiereDetailDto>> CreateAsync(CreateCantiereDto dto)
        {
            if (dto.DataInizio.HasValue && dto.DataFine.HasValue && dto.DataFine.Value < dto.DataInizio.Value)
            {
                return ServiceResult<CantiereDetailDto>.Fail("La data fine non può essere precedente alla data inizio.", 400);
            }

            var nomePulito = dto.Nome.Trim();
            var codicePulito = Pulisci(dto.Codice);

            var nomeEsiste = await _context.Cantieris
                .AnyAsync(c => c.Nome == nomePulito);

            if (nomeEsiste)
            {
                return ServiceResult<CantiereDetailDto>.Fail("Esiste già un cantiere con questo nome.", 400);
            }

            if (!string.IsNullOrWhiteSpace(codicePulito))
            {
                var codiceEsiste = await _context.Cantieris
                    .AnyAsync(c => c.Codice == codicePulito);

                if (codiceEsiste)
                {
                    return ServiceResult<CantiereDetailDto>.Fail("Esiste già un cantiere con questo codice.", 400);
                }
            }

            var cantiere = new Cantieri
            {
                Codice = codicePulito,
                Nome = nomePulito,
                Cliente = Pulisci(dto.Cliente),
                Indirizzo = Pulisci(dto.Indirizzo),
                Citta = Pulisci(dto.Citta),
                Provincia = Pulisci(dto.Provincia),
                Cap = Pulisci(dto.Cap),
                DataInizio = dto.DataInizio,
                DataFine = dto.DataFine,
                Attivo = dto.Attivo,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Cantieris.Add(cantiere);
            await _context.SaveChangesAsync();

            var result = new CantiereDetailDto
            {
                Id = cantiere.Id,
                Codice = cantiere.Codice,
                Nome = cantiere.Nome,
                Cliente = cantiere.Cliente,
                Indirizzo = cantiere.Indirizzo,
                Citta = cantiere.Citta,
                Provincia = cantiere.Provincia,
                Cap = cantiere.Cap,
                DataInizio = cantiere.DataInizio,
                DataFine = cantiere.DataFine,
                Attivo = cantiere.Attivo,
                Note = cantiere.Note
            };

            return ServiceResult<CantiereDetailDto>.Created(result);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCantiereDto dto)
        {
            var cantiere = await _context.Cantieris
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cantiere == null)
            {
                return ServiceResult<bool>.Fail("Cantiere non trovato.", 404);
            }

            if (dto.DataInizio.HasValue && dto.DataFine.HasValue && dto.DataFine.Value < dto.DataInizio.Value)
            {
                return ServiceResult<bool>.Fail("La data fine non può essere precedente alla data inizio.", 400);
            }

            var nomePulito = dto.Nome.Trim();
            var codicePulito = Pulisci(dto.Codice);

            var nomeEsiste = await _context.Cantieris
                .AnyAsync(c => c.Id != id && c.Nome == nomePulito);

            if (nomeEsiste)
            {
                return ServiceResult<bool>.Fail("Esiste già un altro cantiere con questo nome.", 400);
            }

            if (!string.IsNullOrWhiteSpace(codicePulito))
            {
                var codiceEsiste = await _context.Cantieris
                    .AnyAsync(c => c.Id != id && c.Codice == codicePulito);

                if (codiceEsiste)
                {
                    return ServiceResult<bool>.Fail("Esiste già un altro cantiere con questo codice.", 400);
                }
            }

            cantiere.Codice = codicePulito;
            cantiere.Nome = nomePulito;
            cantiere.Cliente = Pulisci(dto.Cliente);
            cantiere.Indirizzo = Pulisci(dto.Indirizzo);
            cantiere.Citta = Pulisci(dto.Citta);
            cantiere.Provincia = Pulisci(dto.Provincia);
            cantiere.Cap = Pulisci(dto.Cap);
            cantiere.DataInizio = dto.DataInizio;
            cantiere.DataFine = dto.DataFine;
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
                      || await _context.MovimentiMateriales.AnyAsync(m => m.CantiereId == id);

            if (usato)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare il cantiere perché è collegato ad altri record.", 400);
            }

            _context.Cantieris.Remove(cantiere);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
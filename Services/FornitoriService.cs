using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class FornitoriService : IFornitoriService
    {
        private readonly AppDbContext _context;

        public FornitoriService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FornitoreListDto>> GetAllAsync()
        {
            return await _context.Fornitoris
                .OrderBy(f => f.RagioneSociale)
                .Select(f => new FornitoreListDto
                {
                    Id = f.Id,
                    RagioneSociale = f.RagioneSociale,
                    PartitaIva = f.PartitaIva,
                    Telefono = f.Telefono,
                    Email = f.Email,
                    Citta = f.Citta,
                    Attivo = f.Attivo
                })
                .ToListAsync();
        }
        public async Task<List<SelectOptionDto>> GetAttiviAsync()
        {
            return await _context.Fornitoris
                .Where(f => f.Attivo)
                .OrderBy(f => f.RagioneSociale)
                .Select(f => new SelectOptionDto
                {
                    Id = f.Id,
                    Nome = f.RagioneSociale
                })
                .ToListAsync();
        }
        public async Task<FornitoreDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Fornitoris
                .Where(f => f.Id == id)
                .Select(f => new FornitoreDetailDto
                {
                    Id = f.Id,
                    RagioneSociale = f.RagioneSociale,
                    PartitaIva = f.PartitaIva,
                    CodiceFiscale = f.CodiceFiscale,
                    Telefono = f.Telefono,
                    Email = f.Email,
                    Indirizzo = f.Indirizzo,
                    Citta = f.Citta,
                    Provincia = f.Provincia,
                    Cap = f.Cap,
                    Note = f.Note,
                    Attivo = f.Attivo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<FornitoreDetailDto>> CreateAsync(CreateFornitoreDto dto)
        {
            var ragioneSocialePulita = dto.RagioneSociale.Trim();

            var esisteGia = await _context.Fornitoris
                .AnyAsync(f => f.RagioneSociale == ragioneSocialePulita);

            if (esisteGia)
            {
                return ServiceResult<FornitoreDetailDto>.Fail("Esiste già un fornitore con questa ragione sociale.", 400);
            }

            var fornitore = new Fornitori
            {
                RagioneSociale = ragioneSocialePulita,
                PartitaIva = Pulisci(dto.PartitaIva),
                CodiceFiscale = Pulisci(dto.CodiceFiscale),
                Telefono = Pulisci(dto.Telefono),
                Email = Pulisci(dto.Email),
                Indirizzo = Pulisci(dto.Indirizzo),
                Citta = Pulisci(dto.Citta),
                Provincia = Pulisci(dto.Provincia),
                Cap = Pulisci(dto.Cap),
                Note = Pulisci(dto.Note),
                Attivo = dto.Attivo,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Fornitoris.Add(fornitore);
            await _context.SaveChangesAsync();

            var result = new FornitoreDetailDto
            {
                Id = fornitore.Id,
                RagioneSociale = fornitore.RagioneSociale,
                PartitaIva = fornitore.PartitaIva,
                CodiceFiscale = fornitore.CodiceFiscale,
                Telefono = fornitore.Telefono,
                Email = fornitore.Email,
                Indirizzo = fornitore.Indirizzo,
                Citta = fornitore.Citta,
                Provincia = fornitore.Provincia,
                Cap = fornitore.Cap,
                Note = fornitore.Note,
                Attivo = fornitore.Attivo
            };

            return ServiceResult<FornitoreDetailDto>.Created(result);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateFornitoreDto dto)
        {
            var fornitore = await _context.Fornitoris
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fornitore == null)
            {
                return ServiceResult<bool>.Fail("Fornitore non trovato.", 404);
            }

            var ragioneSocialePulita = dto.RagioneSociale.Trim();

            var esisteGia = await _context.Fornitoris
                .AnyAsync(f => f.Id != id && f.RagioneSociale == ragioneSocialePulita);

            if (esisteGia)
            {
                return ServiceResult<bool>.Fail("Esiste già un altro fornitore con questa ragione sociale.", 400);
            }

            fornitore.RagioneSociale = ragioneSocialePulita;
            fornitore.PartitaIva = Pulisci(dto.PartitaIva);
            fornitore.CodiceFiscale = Pulisci(dto.CodiceFiscale);
            fornitore.Telefono = Pulisci(dto.Telefono);
            fornitore.Email = Pulisci(dto.Email);
            fornitore.Indirizzo = Pulisci(dto.Indirizzo);
            fornitore.Citta = Pulisci(dto.Citta);
            fornitore.Provincia = Pulisci(dto.Provincia);
            fornitore.Cap = Pulisci(dto.Cap);
            fornitore.Note = Pulisci(dto.Note);
            fornitore.Attivo = dto.Attivo;
            fornitore.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var fornitore = await _context.Fornitoris
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fornitore == null)
            {
                return ServiceResult<bool>.Fail("Fornitore non trovato.", 404);
            }

            var usato = await _context.Mezzis.AnyAsync(m => m.FornitoreId == id)
                      || await _context.Materialis.AnyAsync(m => m.FornitoreId == id)
                      || await _context.Dpis.AnyAsync(d => d.FornitoreId == id);

            if (usato)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare il fornitore perché è collegato ad altri record.", 400);
            }

            _context.Fornitoris.Remove(fornitore);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }
    }
}
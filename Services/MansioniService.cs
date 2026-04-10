using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class MansioniService : IMansioniService
    {
        private readonly AppDbContext _context;

        public MansioniService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MansioneListDto>> GetAllAsync()
        {
            return await _context.Mansionis
                .OrderBy(m => m.Nome)
                .Select(m => new MansioneListDto
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Descrizione = m.Descrizione,
                    Attivo = m.Attivo
                })
                .ToListAsync();
        }
        public async Task<List<SelectOptionDto>> GetAttiveAsync()
        {
            return await _context.Mansionis
                .Where(m => m.Attivo)
                .OrderBy(m => m.Nome)
                .Select(m => new SelectOptionDto
                {
                    Id = m.Id,
                    Nome = m.Nome
                })
                .ToListAsync();
        }

        public async Task<MansioneDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Mansionis
                .Where(m => m.Id == id)
                .Select(m => new MansioneDetailDto
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Descrizione = m.Descrizione,
                    Attivo = m.Attivo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<MansioneDetailDto>> CreateAsync(CreateMansioneDto dto)
        {
            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.Mansionis
                .AnyAsync(m => m.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<MansioneDetailDto>.Fail("Esiste già una mansione con questo nome.");
            }

            var mansione = new Mansioni
            {
                Nome = nomePulito,
                Descrizione = string.IsNullOrWhiteSpace(dto.Descrizione) ? null : dto.Descrizione.Trim(),
                Attivo = dto.Attivo,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Mansionis.Add(mansione);
            await _context.SaveChangesAsync();

            var result = new MansioneDetailDto
            {
                Id = mansione.Id,
                Nome = mansione.Nome,
                Descrizione = mansione.Descrizione,
                Attivo = mansione.Attivo
            };

            return ServiceResult<MansioneDetailDto>.Ok(result);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMansioneDto dto)
        {
            var mansione = await _context.Mansionis
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mansione == null)
            {
                return ServiceResult<bool>.Fail("Mansione non trovata.");
            }

            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.Mansionis
                .AnyAsync(m => m.Id != id && m.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<bool>.Fail("Esiste già un'altra mansione con questo nome.");
            }

            mansione.Nome = nomePulito;
            mansione.Descrizione = string.IsNullOrWhiteSpace(dto.Descrizione) ? null : dto.Descrizione.Trim();
            mansione.Attivo = dto.Attivo;
            mansione.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var mansione = await _context.Mansionis
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mansione == null)
            {
                return ServiceResult<bool>.Fail("Mansione non trovata.");
            }

            var usataDaDipendenti = await _context.Dipendentis
                .AnyAsync(d => d.MansioneId == id);

            if (usataDaDipendenti)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare la mansione perché è assegnata ad uno o più dipendenti.");
            }

            _context.Mansionis.Remove(mansione);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
    }
}
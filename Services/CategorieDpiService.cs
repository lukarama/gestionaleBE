using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class CategorieDpiService : ICategorieDpiService
    {
        private readonly AppDbContext _context;

        public CategorieDpiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaDpiListDto>> GetAllAsync()
        {
            return await _context.CategorieDpis
                .OrderBy(c => c.Nome)
                .Select(c => new CategoriaDpiListDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descrizione = c.Descrizione,
                    Attivo = c.Attivo
                })
                .ToListAsync();
        }

        public async Task<List<SelectOptionDto>> GetAttiveAsync()
        {
            return await _context.CategorieDpis
                .Where(c => c.Attivo)
                .OrderBy(c => c.Nome)
                .Select(c => new SelectOptionDto
                {
                    Id = c.Id,
                    Nome = c.Nome
                })
                .ToListAsync();
        }

        public async Task<CategoriaDpiDetailDto?> GetByIdAsync(int id)
        {
            return await _context.CategorieDpis
                .Where(c => c.Id == id)
                .Select(c => new CategoriaDpiDetailDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descrizione = c.Descrizione,
                    Attivo = c.Attivo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<CategoriaDpiDetailDto>> CreateAsync(CreateCategoriaDpiDto dto)
        {
            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.CategorieDpis
                .AnyAsync(c => c.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<CategoriaDpiDetailDto>.Fail("Esiste già una categoria DPI con questo nome.", 400);
            }

            var categoria = new CategorieDpi
            {
                Nome = nomePulito,
                Descrizione = string.IsNullOrWhiteSpace(dto.Descrizione) ? null : dto.Descrizione.Trim(),
                Attivo = dto.Attivo,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.CategorieDpis.Add(categoria);
            await _context.SaveChangesAsync();

            var result = new CategoriaDpiDetailDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descrizione = categoria.Descrizione,
                Attivo = categoria.Attivo
            };

            return ServiceResult<CategoriaDpiDetailDto>.Created(result);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCategoriaDpiDto dto)
        {
            var categoria = await _context.CategorieDpis
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return ServiceResult<bool>.Fail("Categoria DPI non trovata.", 404);
            }

            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.CategorieDpis
                .AnyAsync(c => c.Id != id && c.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<bool>.Fail("Esiste già un'altra categoria DPI con questo nome.", 400);
            }

            categoria.Nome = nomePulito;
            categoria.Descrizione = string.IsNullOrWhiteSpace(dto.Descrizione) ? null : dto.Descrizione.Trim();
            categoria.Attivo = dto.Attivo;
            categoria.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var categoria = await _context.CategorieDpis
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return ServiceResult<bool>.Fail("Categoria DPI non trovata.", 404);
            }

            var usataDaDpi = await _context.Dpis
                .AnyAsync(d => d.CategoriaDpiId == id);

            if (usataDaDpi)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare la categoria DPI perché è assegnata ad uno o più DPI.", 400);
            }

            _context.CategorieDpis.Remove(categoria);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
    }
}
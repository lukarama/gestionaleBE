using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class CategorieMaterialeService : ICategorieMaterialeService
    {
        private readonly AppDbContext _context;

        public CategorieMaterialeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaMaterialeListDto>> GetAllAsync()
        {
            return await _context.CategorieMateriales
                .OrderBy(c => c.Nome)
                .Select(c => new CategoriaMaterialeListDto
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
            return await _context.CategorieMateriales
                .Where(c => c.Attivo)
                .OrderBy(c => c.Nome)
                .Select(c => new SelectOptionDto
                {
                    Id = c.Id,
                    Nome = c.Nome
                })
                .ToListAsync();
        }

        public async Task<CategoriaMaterialeDetailDto?> GetByIdAsync(int id)
        {
            return await _context.CategorieMateriales
                .Where(c => c.Id == id)
                .Select(c => new CategoriaMaterialeDetailDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descrizione = c.Descrizione,
                    Attivo = c.Attivo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<CategoriaMaterialeDetailDto>> CreateAsync(CreateCategoriaMaterialeDto dto)
        {
            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.CategorieMateriales
                .AnyAsync(c => c.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<CategoriaMaterialeDetailDto>.Fail("Esiste già una categoria materiale con questo nome.", 400);
            }

            var categoria = new CategorieMateriale
            {
                Nome = nomePulito,
                Descrizione = string.IsNullOrWhiteSpace(dto.Descrizione) ? null : dto.Descrizione.Trim(),
                Attivo = dto.Attivo,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.CategorieMateriales.Add(categoria);
            await _context.SaveChangesAsync();

            var result = new CategoriaMaterialeDetailDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descrizione = categoria.Descrizione,
                Attivo = categoria.Attivo
            };

            return ServiceResult<CategoriaMaterialeDetailDto>.Created(result);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCategoriaMaterialeDto dto)
        {
            var categoria = await _context.CategorieMateriales
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return ServiceResult<bool>.Fail("Categoria materiale non trovata.", 404);
            }

            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.CategorieMateriales
                .AnyAsync(c => c.Id != id && c.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<bool>.Fail("Esiste già un'altra categoria materiale con questo nome.", 400);
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
            var categoria = await _context.CategorieMateriales
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
            {
                return ServiceResult<bool>.Fail("Categoria materiale non trovata.", 404);
            }

            var usataDaMateriali = await _context.Materialis
                .AnyAsync(m => m.CategoriaMaterialeId == id);

            if (usataDaMateriali)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare la categoria materiale perché è assegnata ad uno o più materiali.", 400);
            }

            _context.CategorieMateriales.Remove(categoria);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
    }
}
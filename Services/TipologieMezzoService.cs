using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class TipologieMezzoService : ITipologieMezzoService
    {
        private readonly AppDbContext _context;

        public TipologieMezzoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipologiaMezzoListDto>> GetAllAsync()
        {
            return await _context.TipologieMezzos
                .OrderBy(t => t.Nome)
                .Select(t => new TipologiaMezzoListDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Descrizione = t.Descrizione,
                    Attivo = t.Attivo
                })
                .ToListAsync();
        }

        public async Task<TipologiaMezzoDetailDto?> GetByIdAsync(int id)
        {
            return await _context.TipologieMezzos
                .Where(t => t.Id == id)
                .Select(t => new TipologiaMezzoDetailDto
                {
                    Id = t.Id,
                    Nome = t.Nome,
                    Descrizione = t.Descrizione,
                    Attivo = t.Attivo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<TipologiaMezzoDetailDto>> CreateAsync(CreateTipologiaMezzoDto dto)
        {
            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.TipologieMezzos
                .AnyAsync(t => t.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<TipologiaMezzoDetailDto>.Fail("Esiste già una tipologia mezzo con questo nome.", 400);
            }

            var tipologia = new TipologieMezzo
            {
                Nome = nomePulito,
                Descrizione = string.IsNullOrWhiteSpace(dto.Descrizione) ? null : dto.Descrizione.Trim(),
                Attivo = dto.Attivo,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.TipologieMezzos.Add(tipologia);
            await _context.SaveChangesAsync();

            var result = new TipologiaMezzoDetailDto
            {
                Id = tipologia.Id,
                Nome = tipologia.Nome,
                Descrizione = tipologia.Descrizione,
                Attivo = tipologia.Attivo
            };

            return ServiceResult<TipologiaMezzoDetailDto>.Created(result);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateTipologiaMezzoDto dto)
        {
            var tipologia = await _context.TipologieMezzos
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipologia == null)
            {
                return ServiceResult<bool>.Fail("Tipologia mezzo non trovata.", 404);
            }

            var nomePulito = dto.Nome.Trim();

            var esisteGia = await _context.TipologieMezzos
                .AnyAsync(t => t.Id != id && t.Nome == nomePulito);

            if (esisteGia)
            {
                return ServiceResult<bool>.Fail("Esiste già un'altra tipologia mezzo con questo nome.", 400);
            }

            tipologia.Nome = nomePulito;
            tipologia.Descrizione = string.IsNullOrWhiteSpace(dto.Descrizione) ? null : dto.Descrizione.Trim();
            tipologia.Attivo = dto.Attivo;
            tipologia.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
        public async Task<List<SelectOptionDto>> GetAttiveAsync()
        {
            return await _context.TipologieMezzos
                .Where(t => t.Attivo)
                .OrderBy(t => t.Nome)
                .Select(t => new SelectOptionDto
                {
                    Id = t.Id,
                    Nome = t.Nome
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var tipologia = await _context.TipologieMezzos
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipologia == null)
            {
                return ServiceResult<bool>.Fail("Tipologia mezzo non trovata.", 404);
            }

            var usataDaMezzi = await _context.Mezzis
                .AnyAsync(m => m.TipologiaMezzoId == id);

            if (usataDaMezzi)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare la tipologia mezzo perché è assegnata ad uno o più mezzi.", 400);
            }

            _context.TipologieMezzos.Remove(tipologia);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }
    }
}
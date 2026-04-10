using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class TipiMovimentoMaterialeService : ITipiMovimentoMaterialeService
{
    private readonly AppDbContext _context;

    public TipiMovimentoMaterialeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TipiMovimentoMaterialeDto>> GetAllAsync()
    {
        return await _context.TipiMovimentoMateriales
            .OrderBy(x => x.Nome)
            .Select(x => new TipiMovimentoMaterialeDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Descrizione = x.Descrizione,
                Segno = x.Segno,
                Attivo = x.Attivo,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<TipiMovimentoMaterialeDto?> GetByIdAsync(int id)
    {
        return await _context.TipiMovimentoMateriales
            .Where(x => x.Id == id)
            .Select(x => new TipiMovimentoMaterialeDto
            {
                Id = x.Id,
                Nome = x.Nome,
                Descrizione = x.Descrizione,
                Segno = x.Segno,
                Attivo = x.Attivo,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TipiMovimentoMaterialeDto> CreateAsync(CreateTipoMovimentoMaterialeDto dto)
    {
        if (dto.Segno != 1 && dto.Segno != -1)
            throw new Exception("Il campo Segno può essere solo 1 oppure -1.");

        var entity = new TipiMovimentoMateriale
        {
            Nome = dto.Nome,
            Descrizione = dto.Descrizione,
            Segno = dto.Segno,
            Attivo = dto.Attivo,
            CreatedAt = DateTime.Now
        };

        _context.TipiMovimentoMateriales.Add(entity);
        await _context.SaveChangesAsync();

        return new TipiMovimentoMaterialeDto
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Descrizione = entity.Descrizione,
            Segno = entity.Segno,
            Attivo = entity.Attivo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateTipoMovimentoMaterialeDto dto)
    {
        if (dto.Segno != 1 && dto.Segno != -1)
            throw new Exception("Il campo Segno può essere solo 1 oppure -1.");

        var entity = await _context.TipiMovimentoMateriales.FindAsync(id);

        if (entity == null)
            return false;

        entity.Nome = dto.Nome;
        entity.Descrizione = dto.Descrizione;
        entity.Segno = dto.Segno;
        entity.Attivo = dto.Attivo;
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.TipiMovimentoMateriales.FindAsync(id);

        if (entity == null)
            return false;

        entity.Attivo = false;
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }
}
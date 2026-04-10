using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class TipiDocumentoService : ITipiDocumentoService
{
    private readonly AppDbContext _context;

    public TipiDocumentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TipoDocumentoDto>> GetAllAsync()
    {
        return await _context.TipiDocumentos
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .Select(t => new TipoDocumentoDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Descrizione = t.Descrizione,
                Attivo = t.Attivo,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<TipoDocumentoDto?> GetByIdAsync(int id)
    {
        return await _context.TipiDocumentos
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TipoDocumentoDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Descrizione = t.Descrizione,
                Attivo = t.Attivo,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<TipoDocumentoDto>> CreateAsync(CreateTipoDocumentoDto dto)
    {
        var nomePulito = Pulisci(dto.Nome);

        if (string.IsNullOrWhiteSpace(nomePulito))
            return ServiceResult<TipoDocumentoDto>.Fail("Il nome è obbligatorio.", 400);

        var esiste = await _context.TipiDocumentos
            .AnyAsync(t => t.Nome.ToLower() == nomePulito.ToLower());

        if (esiste)
            return ServiceResult<TipoDocumentoDto>.Fail("Esiste già un tipo documento con questo nome.", 400);

        var entity = new TipiDocumento
        {
            Nome = nomePulito,
            Descrizione = Pulisci(dto.Descrizione),
            Attivo = dto.Attivo,
            CreatedAt = DateTime.Now
        };

        _context.TipiDocumentos.Add(entity);
        await _context.SaveChangesAsync();

        var result = new TipoDocumentoDto
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Descrizione = entity.Descrizione,
            Attivo = entity.Attivo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        return ServiceResult<TipoDocumentoDto>.Created(result);
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateTipoDocumentoDto dto)
    {
        var entity = await _context.TipiDocumentos
            .FirstOrDefaultAsync(t => t.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Tipo documento non trovato.", 404);

        var nomePulito = Pulisci(dto.Nome);

        if (string.IsNullOrWhiteSpace(nomePulito))
            return ServiceResult<bool>.Fail("Il nome è obbligatorio.", 400);

        var esiste = await _context.TipiDocumentos
            .AnyAsync(t => t.Id != id && t.Nome.ToLower() == nomePulito.ToLower());

        if (esiste)
            return ServiceResult<bool>.Fail("Esiste già un tipo documento con questo nome.", 400);

        entity.Nome = nomePulito;
        entity.Descrizione = Pulisci(dto.Descrizione);
        entity.Attivo = dto.Attivo;
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var entity = await _context.TipiDocumentos
            .FirstOrDefaultAsync(t => t.Id == id);

        if (entity == null)
            return ServiceResult<bool>.Fail("Tipo documento non trovato.", 404);

        entity.Attivo = false;
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<List<TipoDocumentoSelectDto>> GetSelectAsync()
    {
        return await _context.TipiDocumentos
            .AsNoTracking()
            .Where(t => t.Attivo)
            .OrderBy(t => t.Nome)
            .Select(t => new TipoDocumentoSelectDto
            {
                Id = t.Id,
                Nome = t.Nome,
                Label = t.Nome
            })
            .ToListAsync();
    }

    private static string? Pulisci(string? valore)
    {
        return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
    }
}
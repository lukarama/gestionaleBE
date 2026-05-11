using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services;

public class StatiAssegnazioneService : IStatiAssegnazioneService
{
    private readonly AppDbContext _context;

    public StatiAssegnazioneService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StatoAssegnazioneListDto>> GetAllAsync()
    {
        return await _context.StatiAssegnaziones
            .AsNoTracking()
            .OrderBy(s => s.Nome)
            .Select(s => new StatoAssegnazioneListDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Descrizione = s.Descrizione,
                Attivo = s.Attivo
            })
            .ToListAsync();
    }

    public async Task<List<SelectOptionDto>> GetAttiviAsync()
    {
        return await _context.StatiAssegnaziones
            .AsNoTracking()
            .Where(s => s.Attivo)
            .OrderBy(s => s.Nome)
            .Select(s => new SelectOptionDto
            {
                Id = s.Id,
                Nome = s.Nome
            })
            .ToListAsync();
    }
}

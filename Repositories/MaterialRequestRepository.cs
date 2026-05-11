using Gestionale.Api.Data;
using Gestionale.Api.Models;
using Gestionale.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Repositories;

public class MaterialRequestRepository : IMaterialRequestRepository
{
    private readonly AppDbContext _context;

    public MaterialRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<MaterialRequest> Query()
    {
        return _context.MaterialRequests.AsQueryable();
    }

    public async Task<MaterialRequest?> GetForUpdateAsync(int id)
    {
        return await _context.MaterialRequests.FirstOrDefaultAsync(request => request.Id == id);
    }

    public async Task AddAsync(MaterialRequest request)
    {
        await _context.MaterialRequests.AddAsync(request);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

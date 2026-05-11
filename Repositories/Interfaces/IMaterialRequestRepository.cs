using Gestionale.Api.Models;

namespace Gestionale.Api.Repositories.Interfaces;

public interface IMaterialRequestRepository
{
    IQueryable<MaterialRequest> Query();
    Task<MaterialRequest?> GetForUpdateAsync(int id);
    Task AddAsync(MaterialRequest request);
    Task SaveChangesAsync();
}

using Gestionale.Api.Models;

namespace Gestionale.Api.Repositories.Interfaces;

public interface IIssueReportRepository
{
    IQueryable<IssueReport> Query();
    Task<IssueReport?> GetForUpdateAsync(int id);
    Task AddAsync(IssueReport report);
    Task SaveChangesAsync();
}

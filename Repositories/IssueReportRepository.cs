using Gestionale.Api.Data;
using Gestionale.Api.Models;
using Gestionale.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Repositories;

public class IssueReportRepository : IIssueReportRepository
{
    private readonly AppDbContext _context;

    public IssueReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<IssueReport> Query()
    {
        return _context.IssueReports.AsQueryable();
    }

    public async Task<IssueReport?> GetForUpdateAsync(int id)
    {
        return await _context.IssueReports.FirstOrDefaultAsync(report => report.Id == id);
    }

    public async Task AddAsync(IssueReport report)
    {
        await _context.IssueReports.AddAsync(report);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

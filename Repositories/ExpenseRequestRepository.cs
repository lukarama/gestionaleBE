using Gestionale.Api.Data;
using Gestionale.Api.Models;
using Gestionale.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Repositories;

public class ExpenseRequestRepository : IExpenseRequestRepository
{
    private readonly AppDbContext _context;

    public ExpenseRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<ExpenseRequest> Query()
    {
        return _context.ExpenseRequests.AsQueryable();
    }

    public async Task<ExpenseRequest?> GetForUpdateAsync(int id)
    {
        return await _context.ExpenseRequests.FirstOrDefaultAsync(request => request.Id == id);
    }

    public async Task AddAsync(ExpenseRequest request)
    {
        await _context.ExpenseRequests.AddAsync(request);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

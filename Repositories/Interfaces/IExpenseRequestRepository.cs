using Gestionale.Api.Models;

namespace Gestionale.Api.Repositories.Interfaces;

public interface IExpenseRequestRepository
{
    IQueryable<ExpenseRequest> Query();
    Task<ExpenseRequest?> GetForUpdateAsync(int id);
    Task AddAsync(ExpenseRequest request);
    Task SaveChangesAsync();
}

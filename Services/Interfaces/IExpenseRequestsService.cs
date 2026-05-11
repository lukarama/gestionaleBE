using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IExpenseRequestsService
{
    Task<ServiceResult<List<ExpenseRequestDto>>> GetAllAsync(UserContext user);
    Task<ServiceResult<ExpenseRequestDto>> GetByIdAsync(int id, UserContext user);
    Task<ServiceResult<ExpenseRequestDto>> CreateAsync(CreateExpenseRequestDto dto, UserContext user);
    Task<ServiceResult<ExpenseRequestDto>> UpdateStatusAsync(int id, UpdateRequestStatusDto dto, UserContext user);
    Task<ServiceResult<ExpenseRequestAttachmentDto>> GetAttachmentAsync(int id, UserContext user);
}

using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IMaterialRequestsService
{
    Task<ServiceResult<List<MaterialRequestDto>>> GetAllAsync(UserContext user);
    Task<ServiceResult<MaterialRequestDto>> GetByIdAsync(int id, UserContext user);
    Task<ServiceResult<MaterialRequestDto>> CreateAsync(CreateMaterialRequestDto dto, UserContext user);
    Task<ServiceResult<MaterialRequestDto>> UpdateStatusAsync(int id, UpdateRequestStatusDto dto, UserContext user);
}

using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IDpiService
    {
        Task<List<DpiListDto>> GetAllAsync();
        Task<List<SelectOptionDto>> GetAttiviAsync();
        Task<DpiDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<DpiDetailDto>> CreateAsync(CreateDpiDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDpiDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
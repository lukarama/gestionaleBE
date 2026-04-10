using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface ICategorieDpiService
    {
        Task<List<CategoriaDpiListDto>> GetAllAsync();
        Task<List<SelectOptionDto>> GetAttiveAsync();
        Task<CategoriaDpiDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<CategoriaDpiDetailDto>> CreateAsync(CreateCategoriaDpiDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCategoriaDpiDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}   
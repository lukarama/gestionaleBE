using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface ICategorieMaterialeService
    {
        Task<List<CategoriaMaterialeListDto>> GetAllAsync();
        Task<List<SelectOptionDto>> GetAttiveAsync();
        Task<CategoriaMaterialeDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<CategoriaMaterialeDetailDto>> CreateAsync(CreateCategoriaMaterialeDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCategoriaMaterialeDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
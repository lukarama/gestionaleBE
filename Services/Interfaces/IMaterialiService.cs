using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IMaterialiService
    {
        Task<List<MaterialeListDto>> GetAllAsync();
        Task<List<MaterialeListDto>> GetSottoScortaAsync();
        Task<List<SelectOptionDto>> GetAttiviAsync();
        Task<MaterialeDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<MaterialeDetailDto>> CreateAsync(CreateMaterialeDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMaterialeDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
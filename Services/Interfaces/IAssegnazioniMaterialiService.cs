using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IAssegnazioniMaterialiService
    {
        Task<List<AssegnazioneMaterialeListDto>> GetAllAsync();
        Task<AssegnazioneMaterialeDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<AssegnazioneMaterialeDetailDto>> CreateAsync(CreateAssegnazioneMaterialeDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateAssegnazioneMaterialeDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
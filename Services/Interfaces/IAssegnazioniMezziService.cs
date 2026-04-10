using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IAssegnazioniMezziService
    {
        Task<List<AssegnazioneMezzoListDto>> GetAllAsync();
        Task<AssegnazioneMezzoDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<AssegnazioneMezzoDetailDto>> CreateAsync(CreateAssegnazioneMezzoDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateAssegnazioneMezzoDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
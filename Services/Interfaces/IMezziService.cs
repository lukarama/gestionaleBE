using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IMezziService
    {
        Task<List<MezzoListDto>> GetAllAsync();
        Task<MezzoDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<MezzoDetailDto>> CreateAsync(CreateMezzoDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMezzoDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<List<ScadenzaMezzoDashboardDto>> GetDashboardScadenzeNativeAsync(int giorni = 30);
    }
}
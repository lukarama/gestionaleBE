using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface ITipologieMezzoService
    {
        Task<List<TipologiaMezzoListDto>> GetAllAsync();
        Task<TipologiaMezzoDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<TipologiaMezzoDetailDto>> CreateAsync(CreateTipologiaMezzoDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateTipologiaMezzoDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<List<SelectOptionDto>> GetAttiveAsync();
    }
}
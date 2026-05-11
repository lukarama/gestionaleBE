using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface ICantieriService
    {
        Task<List<CantiereListDto>> GetAllAsync();
        Task<List<SelectOptionDto>> GetAttiviAsync();
        Task<CantiereDetailDto?> GetByIdAsync(int id);
        Task<CantiereSchedaDto?> GetSchedaAsync(int id);
        Task<ServiceResult<CantiereDetailDto>> CreateAsync(CreateCantiereDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCantiereDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}

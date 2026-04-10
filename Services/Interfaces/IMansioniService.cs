using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IMansioniService
    {
        Task<List<MansioneListDto>> GetAllAsync();
        Task<MansioneDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<MansioneDetailDto>> CreateAsync(CreateMansioneDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMansioneDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<List<SelectOptionDto>> GetAttiveAsync();
    }
}
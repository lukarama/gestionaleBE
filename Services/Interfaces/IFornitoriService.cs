using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IFornitoriService
    {
        Task<List<FornitoreListDto>> GetAllAsync();
        Task<FornitoreDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<FornitoreDetailDto>> CreateAsync(CreateFornitoreDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateFornitoreDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<List<SelectOptionDto>> GetAttiviAsync();
    }
}
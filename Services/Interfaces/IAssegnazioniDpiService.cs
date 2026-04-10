using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IAssegnazioniDpiService
    {
        Task<List<AssegnazioneDpiListDto>> GetAllAsync();
        Task<AssegnazioneDpiDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<AssegnazioneDpiDetailDto>> CreateAsync(CreateAssegnazioneDpiDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateAssegnazioneDpiDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
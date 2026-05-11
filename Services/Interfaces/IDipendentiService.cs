using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IDipendentiService
    {
        Task<List<DipendenteListDto>> GetAllAsync();
        Task<DipendenteDetailDto?> GetByIdAsync(int id);
        Task<DipendenteSchedaDto?> GetSchedaAsync(int id);
        Task<ServiceResult<DipendenteDetailDto>> CreateAsync(CreateDipendenteDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDipendenteDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}

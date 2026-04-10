using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces
{
    public interface IMovimentiMaterialeService
    {
        Task<List<MovimentoMaterialeListDto>> GetAllAsync();
        Task<MovimentoMaterialeDetailDto?> GetByIdAsync(int id);
        Task<ServiceResult<MovimentoMaterialeDetailDto>> CreateAsync(CreateMovimentoMaterialeDto dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMovimentoMaterialeDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
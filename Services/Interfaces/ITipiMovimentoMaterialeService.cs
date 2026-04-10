using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface ITipiMovimentoMaterialeService
{
    Task<List<TipiMovimentoMaterialeDto>> GetAllAsync();
    Task<TipiMovimentoMaterialeDto?> GetByIdAsync(int id);
    Task<TipiMovimentoMaterialeDto> CreateAsync(CreateTipoMovimentoMaterialeDto dto);
    Task<bool> UpdateAsync(int id, UpdateTipoMovimentoMaterialeDto dto);
    Task<bool> DeleteAsync(int id);
}
using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface ITipiDocumentoService
{
    Task<List<TipoDocumentoDto>> GetAllAsync();
    Task<TipoDocumentoDto?> GetByIdAsync(int id);
    Task<ServiceResult<TipoDocumentoDto>> CreateAsync(CreateTipoDocumentoDto dto);
    Task<ServiceResult<bool>> UpdateAsync(int id, UpdateTipoDocumentoDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<List<TipoDocumentoSelectDto>> GetSelectAsync();
}
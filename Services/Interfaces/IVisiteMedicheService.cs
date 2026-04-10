using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IVisiteMedicheService
{
    Task<List<VisitaMedicaListDto>> GetAllAsync();
    Task<VisitaMedicaDetailDto?> GetByIdAsync(int id);
    Task<ServiceResult<VisitaMedicaDetailDto>> CreateAsync(CreateVisitaMedicaDto dto);
    Task<ServiceResult<bool>> UpdateAsync(int id, UpdateVisitaMedicaDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);

    Task<List<VisitaMedicaListDto>> GetByDipendenteIdAsync(int dipendenteId);
    Task<List<VisitaMedicaListDto>> GetInScadenzaAsync(int giorni = 30);

    Task<List<TipoVisitaMedicaSelectDto>> GetTipiVisitaSelectAsync();
    Task<List<EsitoVisitaMedicaSelectDto>> GetEsitiSelectAsync();
    Task<List<VisitaMedicaScadenzaDashboardDto>> GetDashboardScadenzeAsync(int giorni = 30);
}
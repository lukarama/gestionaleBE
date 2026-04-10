using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IMagazzinoService
{
    Task<ServiceResult<bool>> PrelevaAsync(PrelievoMaterialeDto dto);
    Task<ServiceResult<bool>> RifornisciAsync(PrelievoMaterialeDto dto);

    Task<ServiceResult<MaterialeScannerDto>> GetByEanAsync(string ean);
    Task<List<MaterialeSottoScortaDto>> GetMaterialiSottoScortaAsync();
    Task<DashboardMagazzinoDto> GetDashboardAsync();
    Task<List<StoricoMovimentoMaterialeDto>> GetStoricoMovimentiMaterialeAsync(int materialeId);
    Task<List<RicercaMaterialeDto>> RicercaMaterialiAsync(string? testo);
    Task<ServiceResult<DettaglioMaterialeDto>> GetMaterialeByIdAsync(int id);
    Task<List<UltimoMovimentoMagazzinoDto>> GetUltimiMovimentiAsync(int top = 10);
    Task<ServiceResult<DisponibilitaMaterialeDto>> GetDisponibilitaMaterialeAsync(int id);
    Task<List<MaterialeSelectDto>> GetMaterialiSelectAsync();
    Task<List<DipendenteSelectDto>> GetDipendentiSelectAsync();
    Task<List<CantiereSelectDto>> GetCantieriSelectAsync();
}
using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IDocumentiMezziService
{
    Task<List<DocumentoMezzoListDto>> GetAllAsync();
    Task<DocumentoMezzoDetailDto?> GetByIdAsync(int id);
    Task<ServiceResult<DocumentoMezzoDetailDto>> CreateAsync(CreateDocumentoMezzoDto dto);
    Task<ServiceResult<DocumentoMezzoDetailDto>> UploadAsync(UploadDocumentoMezzoDto dto);
    Task<ServiceResult<DocumentoMezzoDownloadDto>> GetDownloadAsync(int id);
    Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDocumentoMezzoDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);

    Task<List<DocumentoMezzoListDto>> GetByMezzoIdAsync(int mezzoId);
    Task<List<DocumentoMezzoScadenzaDto>> GetInScadenzaAsync(int giorni = 30);
    Task<List<DocumentoMezzoDashboardScadenzaDto>> GetDashboardScadenzeAsync(int giorni = 30);
}
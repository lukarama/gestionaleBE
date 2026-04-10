using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IDocumentiDipendentiService
{
    Task<List<DocumentoDipendenteListDto>> GetAllAsync();
    Task<DocumentoDipendenteDetailDto?> GetByIdAsync(int id);
    Task<ServiceResult<DocumentoDipendenteDetailDto>> CreateAsync(CreateDocumentoDipendenteDto dto);
    Task<ServiceResult<DocumentoDipendenteDetailDto>> UploadAsync(UploadDocumentoDipendenteDto dto);
    Task<ServiceResult<DocumentoDipendenteDownloadDto>> GetDownloadAsync(int id);
    Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDocumentoDipendenteDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);

    Task<List<DocumentoDipendenteListDto>> GetByDipendenteIdAsync(int dipendenteId);
    Task<List<DocumentoDipendenteScadenzaDto>> GetInScadenzaAsync(int giorni = 30);
}
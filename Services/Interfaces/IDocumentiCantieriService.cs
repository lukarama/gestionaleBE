using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IDocumentiCantieriService
{
    Task<List<DocumentoCantiereListDto>> GetAllAsync();
    Task<DocumentoCantiereDetailDto?> GetByIdAsync(int id);
    Task<List<DocumentoCantiereListDto>> GetByCantiereIdAsync(int cantiereId);
    Task<ServiceResult<DocumentoCantiereDetailDto>> CreateAsync(CreateDocumentoCantiereDto dto);
    Task<ServiceResult<DocumentoCantiereDetailDto>> UploadAsync(UploadDocumentoCantiereDto dto);
    Task<ServiceResult<DocumentoCantiereDownloadDto>> GetDownloadAsync(int id);
    Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDocumentoCantiereDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}

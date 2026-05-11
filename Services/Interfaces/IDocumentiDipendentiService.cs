using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IDocumentiDipendentiService
{
    Task<List<DocumentoDipendenteListDto>> GetAllAsync();
    Task<DocumentoDipendenteDetailDto?> GetByIdAsync(int id);
    Task<ServiceResult<DocumentoDipendenteDetailDto>> CreateAsync(CreateDocumentoDipendenteDto dto);
    Task<ServiceResult<DocumentoDipendenteDetailDto>> UploadAsync(UploadDocumentoDipendenteDto dto, UserContext user);
    Task<ServiceResult<DocumentoDipendenteDownloadDto>> GetDownloadAsync(int id, UserContext user);
    Task<ServiceResult<DocumentoDipendenteDownloadDto>> GetFileDownloadByDipendenteIdAsync(int dipendenteId, string nomeFile);
    Task<ServiceResult<bool>> UpdateAsync(int id, UpdateDocumentoDipendenteDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<bool>> DeleteAsync(int id, UserContext user);
    Task<ServiceResult<bool>> RenameDocumentoAsync(int id, RenameDocumentoDipendenteDto dto, UserContext user);

    Task<List<DocumentoDipendenteListDto>> GetByDipendenteIdAsync(int dipendenteId);
    Task<ServiceResult<List<DocumentoDipendenteFileDto>>> GetFilesByDipendenteIdAsync(int dipendenteId);
    Task<List<DocumentoDipendenteScadenzaDto>> GetInScadenzaAsync(int giorni = 30);
    Task<ServiceResult<List<DipendenteSelectDto>>> GetDipendentiDocumentiSelectAsync(UserContext user);
    Task<ServiceResult<DocumentiDipendenteTreeDto>> GetTreeByDipendenteIdAsync(int dipendenteId, UserContext user);
    Task<ServiceResult<DocumentiDipendenteTreeDto>> GetMyTreeAsync(UserContext user);
    Task<ServiceResult<CartellaDocumentoDipendenteDto>> CreateCartellaAsync(int dipendenteId, CreateCartellaDocumentoDipendenteDto dto, UserContext user);
    Task<ServiceResult<CartellaDocumentoDipendenteDto>> RenameCartellaAsync(int cartellaId, UpdateCartellaDocumentoDipendenteDto dto, UserContext user);
    Task<ServiceResult<bool>> DeleteCartellaAsync(int cartellaId, UserContext user);
}

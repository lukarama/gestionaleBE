using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IImportazioniMagazzinoService
{
    Task<ServiceResult<ImportMovimentiMagazzinoPreviewDto>> CreaAnteprimaAsync(UploadImportMovimentiMagazzinoDto dto);
    Task<ServiceResult<ImportMovimentiMagazzinoResultDto>> ConfermaImportazioneAsync(ConfermaImportMovimentiMagazzinoDto dto);
}

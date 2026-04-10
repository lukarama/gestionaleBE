using Microsoft.AspNetCore.Http;

namespace Gestionale.Api.DTOs;

public class UploadDocumentoMezzoDto
{
    public int MezzoId { get; set; }
    public int? TipoDocumentoId { get; set; }
    public DateOnly? DataDocumento { get; set; }
    public DateOnly? DataScadenza { get; set; }
    public string? Note { get; set; }
    public IFormFile File { get; set; } = null!;
}
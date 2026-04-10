using Microsoft.AspNetCore.Http;

namespace Gestionale.Api.DTOs;

public class UploadDocumentoDipendenteDto
{
    public int DipendenteId { get; set; }
    public int? TipoDocumentoId { get; set; }
    public DateOnly? DataDocumento { get; set; }
    public DateOnly? DataScadenza { get; set; }
    public string? Note { get; set; }
    public IFormFile File { get; set; } = null!;
}
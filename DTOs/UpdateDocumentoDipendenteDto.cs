namespace Gestionale.Api.DTOs;

public class UpdateDocumentoDipendenteDto
{
    public int DipendenteId { get; set; }
    public int? CartellaId { get; set; }
    public int? TipoDocumentoId { get; set; }
    public string NomeFile { get; set; } = null!;
    public string PercorsoFile { get; set; } = null!;
    public string? Estensione { get; set; }
    public string? ContentType { get; set; }
    public DateOnly? DataDocumento { get; set; }
    public DateOnly? DataScadenza { get; set; }
    public string? Note { get; set; }
}

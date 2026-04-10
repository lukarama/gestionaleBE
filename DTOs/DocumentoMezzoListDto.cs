namespace Gestionale.Api.DTOs;

public class DocumentoMezzoListDto
{
    public int Id { get; set; }
    public int MezzoId { get; set; }
    public string Mezzo { get; set; } = null!;
    public int? TipoDocumentoId { get; set; }
    public string? TipoDocumento { get; set; }
    public string NomeFile { get; set; } = null!;
    public string PercorsoFile { get; set; } = null!;
    public string? Estensione { get; set; }
    public string? ContentType { get; set; }
    public DateOnly? DataDocumento { get; set; }
    public DateOnly? DataScadenza { get; set; }
}
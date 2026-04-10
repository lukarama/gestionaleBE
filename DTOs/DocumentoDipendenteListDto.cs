namespace Gestionale.Api.DTOs;

public class DocumentoDipendenteListDto
{
    public int Id { get; set; }
    public int DipendenteId { get; set; }
    public string Dipendente { get; set; } = null!;
    public int? TipoDocumentoId { get; set; }
    public string? TipoDocumento { get; set; }
    public string NomeFile { get; set; } = null!;
    public string PercorsoFile { get; set; } = null!;
    public string? Estensione { get; set; }
    public string? ContentType { get; set; }
    public DateOnly? DataDocumento { get; set; }
    public DateOnly? DataScadenza { get; set; }
}
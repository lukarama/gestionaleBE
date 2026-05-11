namespace Gestionale.Api.DTOs;

public class DocumentoCantiereListDto
{
    public int Id { get; set; }
    public int CantiereId { get; set; }
    public string Cantiere { get; set; } = string.Empty;
    public string NomeFile { get; set; } = string.Empty;
    public string PercorsoFile { get; set; } = string.Empty;
    public string? Estensione { get; set; }
    public string? ContentType { get; set; }
    public DateOnly? DataDocumento { get; set; }
}

namespace Gestionale.Api.DTOs;

public class DocumentoMezzoScadenzaDto
{
    public int Id { get; set; }
    public int MezzoId { get; set; }
    public string Mezzo { get; set; } = null!;
    public int? TipoDocumentoId { get; set; }
    public string? TipoDocumento { get; set; }
    public string NomeFile { get; set; } = null!;
    public DateOnly? DataScadenza { get; set; }
    public int GiorniAllaScadenza { get; set; }
    public bool Scaduto { get; set; }
    public bool InScadenza { get; set; }
}
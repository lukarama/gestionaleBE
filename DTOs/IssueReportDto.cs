namespace Gestionale.Api.DTOs;

public class IssueReportDto
{
    public int Id { get; set; }
    public int DipendenteId { get; set; }
    public string Dipendente { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Oggetto { get; set; } = string.Empty;
    public string? Luogo { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public string Priorita { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string Stato { get; set; } = string.Empty;
    public string? NotaGestione { get; set; }
    public int? GestitoDaUtenteId { get; set; }
    public string? GestitoDaUtente { get; set; }
    public DateTime? GestitoAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

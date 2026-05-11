namespace Gestionale.Api.DTOs;

public class CreateIssueReportDto
{
    public int? DipendenteId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Oggetto { get; set; } = string.Empty;
    public string? Luogo { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public string Priorita { get; set; } = string.Empty;
    public string? Note { get; set; }
}

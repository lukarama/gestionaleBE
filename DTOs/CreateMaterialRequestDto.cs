namespace Gestionale.Api.DTOs;

public class CreateMaterialRequestDto
{
    public int? DipendenteId { get; set; }
    public string MaterialeRichiesto { get; set; } = string.Empty;
    public decimal Quantita { get; set; }
    public string Motivazione { get; set; } = string.Empty;
    public string Priorita { get; set; } = string.Empty;
    public DateOnly DataDesiderata { get; set; }
    public string? Note { get; set; }
}

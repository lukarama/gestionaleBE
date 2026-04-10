namespace Gestionale.Api.DTOs;

public class MaterialeSelectDto
{
    public int Id { get; set; }
    public string Label { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string? Codice { get; set; }
    public string? Barcode { get; set; }
    public decimal QuantitaAttuale { get; set; }
    public string? UnitaMisura { get; set; }
}
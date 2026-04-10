namespace Gestionale.Api.DTOs;

public class RicercaMaterialeDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Codice { get; set; }
    public string? Barcode { get; set; }
    public string? Categoria { get; set; }
    public string? UnitaMisura { get; set; }
    public decimal QuantitaAttuale { get; set; }
    public decimal ScortaMinima { get; set; }
    public bool SottoScorta { get; set; }
    public bool Attivo { get; set; }
}
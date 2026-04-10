namespace Gestionale.Api.DTOs;

public class MaterialeScannerDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? CodiceInterno { get; set; }
    public string? Ean { get; set; }
    public decimal QuantitaAttuale { get; set; }
    public decimal ScortaMinima { get; set; }
    public bool SottoScorta { get; set; }
    public string? Categoria { get; set; }
    public string? UnitaMisura { get; set; }
}
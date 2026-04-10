namespace Gestionale.Api.DTOs;

public class StoricoMovimentoMaterialeDto
{
    public int Id { get; set; }
    public DateTime DataMovimento { get; set; }
    public string TipoMovimento { get; set; } = null!;
    public short Segno { get; set; }
    public decimal Quantita { get; set; }
    public string? Dipendente { get; set; }
    public string? Cantiere { get; set; }
    public string? RiferimentoTabella { get; set; }
    public int? RiferimentoId { get; set; }
    public string? Note { get; set; }
}
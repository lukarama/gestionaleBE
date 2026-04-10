namespace Gestionale.Api.DTOs;

public class UpdateTipoMovimentoMaterialeDto
{
    public string Nome { get; set; } = null!;
    public string? Descrizione { get; set; }
    public short Segno { get; set; }
    public bool Attivo { get; set; }
}
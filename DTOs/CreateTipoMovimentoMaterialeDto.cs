namespace Gestionale.Api.DTOs;

public class CreateTipoMovimentoMaterialeDto
{
    public string Nome { get; set; } = null!;
    public string? Descrizione { get; set; }
    public short Segno { get; set; }
    public bool Attivo { get; set; } = true;
}
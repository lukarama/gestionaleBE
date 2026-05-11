namespace Gestionale.Api.DTOs;

public class TipoAssenzaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descrizione { get; set; }
    public bool Attivo { get; set; }
}

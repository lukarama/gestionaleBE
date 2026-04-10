namespace Gestionale.Api.DTOs;

public class TipoDocumentoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Descrizione { get; set; }
    public bool Attivo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
namespace Gestionale.Api.DTOs;

public class UpdateTipoDocumentoDto
{
    public string Nome { get; set; } = null!;
    public string? Descrizione { get; set; }
    public bool Attivo { get; set; }
}
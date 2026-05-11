namespace Gestionale.Api.DTOs;

public class CreateAssenzaDto
{
    public int DipendenteId { get; set; }
    public int TipoAssenzaId { get; set; }
    public DateOnly DataInizio { get; set; }
    public DateOnly DataFine { get; set; }
    public string? Note { get; set; }
}

namespace Gestionale.Api.DTOs;

public class AssenzaListDto
{
    public int Id { get; set; }
    public int DipendenteId { get; set; }
    public string Dipendente { get; set; } = null!;
    public int TipoAssenzaId { get; set; }
    public string TipoAssenza { get; set; } = null!;
    public DateOnly DataInizio { get; set; }
    public DateOnly DataFine { get; set; }
    public int Giorni { get; set; }
    public string Stato { get; set; } = null!;
    public string? Note { get; set; }
    public DateTime DataRichiesta { get; set; }
}

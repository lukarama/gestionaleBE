namespace Gestionale.Api.DTOs;

public class VisitaMedicaScadenzaDashboardDto
{
    public int Id { get; set; }
    public int DipendenteId { get; set; }
    public string Dipendente { get; set; } = null!;
    public int TipoVisitaMedicaId { get; set; }
    public string TipoVisitaMedica { get; set; } = null!;
    public DateOnly DataVisita { get; set; }
    public DateOnly? DataScadenza { get; set; }
    public int GiorniAllaScadenza { get; set; }
    public bool Scaduta { get; set; }
    public bool InScadenza { get; set; }
    public bool? Idoneo { get; set; }
    public string? EsitoVisitaMedica { get; set; }
}
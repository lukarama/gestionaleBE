namespace Gestionale.Api.DTOs;

public class VisitaMedicaListDto
{
    public int Id { get; set; }
    public int DipendenteId { get; set; }
    public string Dipendente { get; set; } = null!;
    public int TipoVisitaMedicaId { get; set; }
    public string TipoVisitaMedica { get; set; } = null!;
    public DateOnly DataVisita { get; set; }
    public DateOnly? DataScadenza { get; set; }
    public int? EsitoVisitaMedicaId { get; set; }
    public string? EsitoVisitaMedica { get; set; }
    public bool? Idoneo { get; set; }
    public string? MedicoCompetente { get; set; }
    public string? StrutturaSanitaria { get; set; }
}
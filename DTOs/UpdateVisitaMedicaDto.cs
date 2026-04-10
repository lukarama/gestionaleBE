namespace Gestionale.Api.DTOs;

public class UpdateVisitaMedicaDto
{
    public int DipendenteId { get; set; }
    public int TipoVisitaMedicaId { get; set; }
    public DateOnly DataVisita { get; set; }
    public DateOnly? DataScadenza { get; set; }
    public int? EsitoVisitaMedicaId { get; set; }
    public bool? Idoneo { get; set; }
    public string? Prescrizioni { get; set; }
    public string? MedicoCompetente { get; set; }
    public string? StrutturaSanitaria { get; set; }
    public string? Note { get; set; }
}
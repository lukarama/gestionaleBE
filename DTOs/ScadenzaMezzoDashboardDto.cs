namespace Gestionale.Api.DTOs;

public class ScadenzaMezzoDashboardDto
{
    public int MezzoId { get; set; }
    public string Mezzo { get; set; } = null!;
    public string TipoScadenza { get; set; } = null!;
    public DateOnly DataScadenza { get; set; }
    public int GiorniAllaScadenza { get; set; }
    public bool Scaduta { get; set; }
    public bool InScadenza { get; set; }
}
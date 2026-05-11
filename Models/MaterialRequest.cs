namespace Gestionale.Api.Models;

public partial class MaterialRequest
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public string MaterialeRichiesto { get; set; } = null!;

    public decimal Quantita { get; set; }

    public string Motivazione { get; set; } = null!;

    public string Priorita { get; set; } = null!;

    public DateOnly DataDesiderata { get; set; }

    public string? Note { get; set; }

    public string Stato { get; set; } = null!;

    public string? NotaGestione { get; set; }

    public int? GestitoDaUtenteId { get; set; }

    public DateTime? GestitoAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual Utenti? GestitoDaUtente { get; set; }
}

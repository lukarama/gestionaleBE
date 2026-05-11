namespace Gestionale.Api.Models;

public partial class IssueReport
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public string Categoria { get; set; } = null!;

    public string Oggetto { get; set; } = null!;

    public string? Luogo { get; set; }

    public string Descrizione { get; set; } = null!;

    public string Priorita { get; set; } = null!;

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

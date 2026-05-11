namespace Gestionale.Api.Models;

public partial class ExpenseRequest
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public DateOnly DataSpesa { get; set; }

    public string CategoriaSpesa { get; set; } = null!;

    public string Descrizione { get; set; } = null!;

    public decimal Importo { get; set; }

    public string Valuta { get; set; } = null!;

    public string MetodoPagamento { get; set; } = null!;

    public string Stato { get; set; } = null!;

    public string? AllegatoNomeFile { get; set; }

    public string? AllegatoPercorsoFile { get; set; }

    public string? AllegatoContentType { get; set; }

    public string? AllegatoEstensione { get; set; }

    public string? NotaGestione { get; set; }

    public int? GestitoDaUtenteId { get; set; }

    public DateTime? GestitoAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual Utenti? GestitoDaUtente { get; set; }
}

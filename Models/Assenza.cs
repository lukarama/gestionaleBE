using System;

namespace Gestionale.Api.Models;

public partial class Assenza
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public int TipoAssenzaId { get; set; }

    public DateOnly DataInizio { get; set; }

    public DateOnly DataFine { get; set; }

    public int Giorni { get; set; }

    public string? Note { get; set; }

    public DateTime DataRichiesta { get; set; }

    public string Stato { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual TipoAssenza TipoAssenza { get; set; } = null!;
}

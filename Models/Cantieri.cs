using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Cantieri
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Indirizzo { get; set; }
    public string? ResponsabileCantiere { get; set; }

    public string? DirezioneLavori { get; set; }

    public string? Committente { get; set; }

    public string? Appaltatore { get; set; }

    public DateOnly? DataInizioLavori { get; set; }

    public DateOnly? DataPrevistaFineLavori { get; set; }

    public bool Attivo { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssegnazioniDpi> AssegnazioniDpis { get; set; } = new List<AssegnazioniDpi>();

    public virtual ICollection<AssegnazioniMateriali> AssegnazioniMaterialis { get; set; } = new List<AssegnazioniMateriali>();

    public virtual ICollection<AssegnazioniMezzi> AssegnazioniMezzis { get; set; } = new List<AssegnazioniMezzi>();

    public virtual ICollection<DocumentiCantieri> DocumentiCantieris { get; set; } = new List<DocumentiCantieri>();

    public virtual ICollection<MovimentiMateriale> MovimentiMateriales { get; set; } = new List<MovimentiMateriale>();
}

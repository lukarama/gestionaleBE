using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class StatiAssegnazione
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descrizione { get; set; }

    public bool Attivo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssegnazioniDpi> AssegnazioniDpis { get; set; } = new List<AssegnazioniDpi>();

    public virtual ICollection<AssegnazioniMateriali> AssegnazioniMaterialis { get; set; } = new List<AssegnazioniMateriali>();

    public virtual ICollection<AssegnazioniMezzi> AssegnazioniMezzis { get; set; } = new List<AssegnazioniMezzi>();
}

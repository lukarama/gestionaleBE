using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Materiali
{
    public int Id { get; set; }

    public string? Codice { get; set; }

    public string Nome { get; set; } = null!;

    public int? CategoriaMaterialeId { get; set; }

    public string? Descrizione { get; set; }

    public string? UnitaMisura { get; set; }

    public decimal QuantitaAttuale { get; set; }

    public decimal? ScortaMinima { get; set; }

    public string? Barcode { get; set; }

    public int? FornitoreId { get; set; }

    public bool Attivo { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssegnazioniMateriali> AssegnazioniMaterialis { get; set; } = new List<AssegnazioniMateriali>();

    public virtual CategorieMateriale? CategoriaMateriale { get; set; }

    public virtual Fornitori? Fornitore { get; set; }

    public virtual ICollection<MovimentiMateriale> MovimentiMateriales { get; set; } = new List<MovimentiMateriale>();
}

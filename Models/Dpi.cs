using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Dpi
{
    public int Id { get; set; }

    public string? Codice { get; set; }

    public string Nome { get; set; } = null!;

    public int? CategoriaDpiId { get; set; }

    public string? Descrizione { get; set; }

    public string? Taglia { get; set; }

    public string? Marca { get; set; }

    public string? Modello { get; set; }

    public string? Barcode { get; set; }

    public int? FornitoreId { get; set; }

    public int? DurataGiorni { get; set; }

    public decimal QuantitaDisponibile { get; set; }

    public decimal QuantitaMinima { get; set; }

    public bool RichiedeTaglia { get; set; }

    public bool HaScadenza { get; set; }

    public bool Attivo { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssegnazioniDpi> AssegnazioniDpis { get; set; } = new List<AssegnazioniDpi>();

    public virtual CategorieDpi? CategoriaDpi { get; set; }

    public virtual Fornitori? Fornitore { get; set; }
}

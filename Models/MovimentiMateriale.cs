using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class MovimentiMateriale
{
    public int Id { get; set; }

    public int MaterialeId { get; set; }

    public int TipoMovimentoMaterialeId { get; set; }

    public decimal Quantita { get; set; }

    public DateTime DataMovimento { get; set; }

    public int? DipendenteId { get; set; }

    public int? CantiereId { get; set; }

    public string? RiferimentoTabella { get; set; }

    public int? RiferimentoId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Cantieri? Cantiere { get; set; }

    public virtual Dipendenti? Dipendente { get; set; }

    public virtual Materiali Materiale { get; set; } = null!;

    public virtual TipiMovimentoMateriale TipoMovimentoMateriale { get; set; } = null!;
}

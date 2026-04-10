using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class AssegnazioniMateriali
{
    public int Id { get; set; }

    public int MaterialeId { get; set; }

    public int? DipendenteId { get; set; }

    public int? CantiereId { get; set; }

    public decimal Quantita { get; set; }

    public int StatoAssegnazioneId { get; set; }

    public DateTime DataAssegnazione { get; set; }

    public DateTime? DataRestituzione { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Cantieri? Cantiere { get; set; }

    public virtual Dipendenti? Dipendente { get; set; }

    public virtual Materiali Materiale { get; set; } = null!;

    public virtual StatiAssegnazione StatoAssegnazione { get; set; } = null!;
}

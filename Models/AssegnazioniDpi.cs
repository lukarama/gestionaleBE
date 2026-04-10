using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class AssegnazioniDpi
{
    public int Id { get; set; }

    public int DipendenteId { get; set; }

    public int DpiId { get; set; }

    public int? CantiereId { get; set; }

    public int Quantita { get; set; }

    public int StatoAssegnazioneId { get; set; }

    public DateOnly DataConsegna { get; set; }

    public DateOnly? DataScadenza { get; set; }

    public DateOnly? DataRestituzione { get; set; }

    public bool FirmaConsegna { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Cantieri? Cantiere { get; set; }

    public virtual Dipendenti Dipendente { get; set; } = null!;

    public virtual Dpi Dpi { get; set; } = null!;

    public virtual StatiAssegnazione StatoAssegnazione { get; set; } = null!;
}

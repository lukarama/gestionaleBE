using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class AssegnazioniMezzi
{
    public int Id { get; set; }

    public int MezzoId { get; set; }

    public int? DipendenteId { get; set; }

    public int? CantiereId { get; set; }

    public int StatoAssegnazioneId { get; set; }

    public DateTime DataInizio { get; set; }

    public DateTime? DataFine { get; set; }

    public int? KmConsegna { get; set; }

    public int? KmRientro { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Cantieri? Cantiere { get; set; }

    public virtual Dipendenti? Dipendente { get; set; }

    public virtual Mezzi Mezzo { get; set; } = null!;

    public virtual StatiAssegnazione StatoAssegnazione { get; set; } = null!;
}

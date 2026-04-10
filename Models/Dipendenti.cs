using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Dipendenti
{
    public int Id { get; set; }

    public string? Matricola { get; set; }

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public string? CodiceFiscale { get; set; }

    public DateOnly? DataNascita { get; set; }

    public string? LuogoNascita { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Indirizzo { get; set; }

    public string? Citta { get; set; }

    public string? Provincia { get; set; }

    public string? Cap { get; set; }

    public DateOnly? DataAssunzione { get; set; }

    public DateOnly? DataCessazione { get; set; }

    public int? MansioneId { get; set; }

    public bool Attivo { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssegnazioniDpi> AssegnazioniDpis { get; set; } = new List<AssegnazioniDpi>();

    public virtual ICollection<AssegnazioniMateriali> AssegnazioniMaterialis { get; set; } = new List<AssegnazioniMateriali>();

    public virtual ICollection<AssegnazioniMezzi> AssegnazioniMezzis { get; set; } = new List<AssegnazioniMezzi>();

    public virtual ICollection<DocumentiDipendenti> DocumentiDipendentis { get; set; } = new List<DocumentiDipendenti>();

    public virtual Mansioni? Mansione { get; set; }

    public virtual ICollection<MovimentiMateriale> MovimentiMateriales { get; set; } = new List<MovimentiMateriale>();

    public virtual ICollection<VisiteMediche> VisiteMediches { get; set; } = new List<VisiteMediche>();
}

using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Mezzi
{
    public int Id { get; set; }

    public string? Targa { get; set; }

    public string? NumeroTelaio { get; set; }

    public string? CodiceInterno { get; set; }

    public int? TipologiaMezzoId { get; set; }

    public string? Marca { get; set; }

    public string? Modello { get; set; }

    public int? AnnoImmatricolazione { get; set; }

    public DateOnly? DataImmatricolazione { get; set; }

    public DateOnly? DataRevisione { get; set; }

    public DateOnly? DataScadenzaBollo { get; set; }

    public DateOnly? DataScadenzaAssicurazione { get; set; }

    public DateOnly? DataTagliando { get; set; }

    public int? FornitoreId { get; set; }

    public bool Attivo { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssegnazioniMezzi> AssegnazioniMezzis { get; set; } = new List<AssegnazioniMezzi>();

    public virtual ICollection<DocumentiMezzi> DocumentiMezzis { get; set; } = new List<DocumentiMezzi>();

    public virtual Fornitori? Fornitore { get; set; }

    public virtual TipologieMezzo? TipologiaMezzo { get; set; }
}

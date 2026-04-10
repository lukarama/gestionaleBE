using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Fornitori
{
    public int Id { get; set; }

    public string RagioneSociale { get; set; } = null!;

    public string? PartitaIva { get; set; }

    public string? CodiceFiscale { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Indirizzo { get; set; }

    public string? Citta { get; set; }

    public string? Provincia { get; set; }

    public string? Cap { get; set; }

    public string? Note { get; set; }

    public bool Attivo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Dpi> Dpis { get; set; } = new List<Dpi>();

    public virtual ICollection<Materiali> Materialis { get; set; } = new List<Materiali>();

    public virtual ICollection<Mezzi> Mezzis { get; set; } = new List<Mezzi>();
}

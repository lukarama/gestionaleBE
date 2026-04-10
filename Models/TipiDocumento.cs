using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class TipiDocumento
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descrizione { get; set; }

    public bool Attivo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<DocumentiDipendenti> DocumentiDipendentis { get; set; } = new List<DocumentiDipendenti>();

    public virtual ICollection<DocumentiMezzi> DocumentiMezzis { get; set; } = new List<DocumentiMezzi>();
}

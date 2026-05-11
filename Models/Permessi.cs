using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Permessi
{
    public int Id { get; set; }

    public string Codice { get; set; } = null!;

    public string Risorsa { get; set; } = null!;

    public string Azione { get; set; } = null!;

    public string? Descrizione { get; set; }

    public bool Attivo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<RuoliPermessi> RuoliPermessis { get; set; } = new List<RuoliPermessi>();
}

using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Mansioni
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descrizione { get; set; }

    public bool Attivo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Dipendenti> Dipendentis { get; set; } = new List<Dipendenti>();
}

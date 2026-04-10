using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class TipiMovimentoMateriale
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descrizione { get; set; }

    public short Segno { get; set; }

    public bool Attivo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MovimentiMateriale> MovimentiMateriales { get; set; } = new List<MovimentiMateriale>();
}

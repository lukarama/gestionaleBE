using System;

namespace Gestionale.Api.Models;

public partial class RuoliPermessi
{
    public int RuoloId { get; set; }

    public int PermessoId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Permessi Permesso { get; set; } = null!;

    public virtual Ruoli Ruolo { get; set; } = null!;
}

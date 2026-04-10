using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class UtentiRuoli
{
    public int UtenteId { get; set; }

    public int RuoloId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Ruoli Ruolo { get; set; } = null!;

    public virtual Utenti Utente { get; set; } = null!;
}

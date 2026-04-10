using System;
using System.Collections.Generic;

namespace Gestionale.Api.Models;

public partial class Utenti
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public bool Attivo { get; set; }

    public DateTime? UltimoAccessoAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<UtentiRuoli> UtentiRuolis { get; set; } = new List<UtentiRuoli>();
}

using System;

namespace Gestionale.Api.Models;

public partial class UtentiVisibilita
{
    public int Id { get; set; }

    public int UtenteId { get; set; }

    public string Chiave { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Utenti Utente { get; set; } = null!;
}

using System;

namespace Gestionale.Api.Models;

public partial class RefreshTokens
{
    public int Id { get; set; }

    public int UtenteId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public string? UserAgent { get; set; }

    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;

    public virtual Utenti Utente { get; set; } = null!;
}

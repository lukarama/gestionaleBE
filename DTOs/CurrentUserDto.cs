using System.Collections.Generic;

namespace Gestionale.Api.DTOs;

public class CurrentUserDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Cognome { get; set; } = string.Empty;

    public int? DipendenteId { get; set; }

    public bool MustChangePassword { get; set; }

    public IReadOnlyCollection<string> Roles { get; set; } = [];

    public IReadOnlyCollection<string> Permissions { get; set; } = [];
}

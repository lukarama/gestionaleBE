using System.Collections.Generic;

namespace Gestionale.Api.DTOs;

public class AuthUserProfileDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string NomeCompleto { get; set; } = string.Empty;

    public int? DipendenteId { get; set; }

    public IReadOnlyCollection<string> Roles { get; set; } = [];

    public IReadOnlyCollection<string> Permissions { get; set; } = [];
}

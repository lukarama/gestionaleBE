namespace Gestionale.Api.DTOs;

public class UtenteDipendenteAdminDto
{
    public int DipendenteId { get; set; }

    public string DipendenteNome { get; set; } = string.Empty;

    public string DipendenteCognome { get; set; } = string.Empty;

    public string? DipendenteEmail { get; set; }

    public string? DipendenteMatricola { get; set; }

    public bool DipendenteAttivo { get; set; }

    public int? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public bool? AccountAttivo { get; set; }

    public string[] Roles { get; set; } = [];

    public string[] Visibility { get; set; } = [];

    public DateTime? LastLoginAtUtc { get; set; }
}

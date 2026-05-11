namespace Gestionale.Api.DTOs;

public class CreateUtenteDipendenteDto
{
    public int DipendenteId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string TemporaryPassword { get; set; } = string.Empty;

    public string[] Roles { get; set; } = [];

    public string[] Visibility { get; set; } = [];
}

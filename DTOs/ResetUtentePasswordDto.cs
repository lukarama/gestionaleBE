namespace Gestionale.Api.DTOs;

public class ResetUtentePasswordDto
{
    public string? TemporaryPassword { get; set; }

    public bool? MustChangePassword { get; set; }
}

namespace Gestionale.Api.DTOs;

public class ResetUtentePasswordResultDto
{
    public string TemporaryPassword { get; set; } = string.Empty;

    public bool MustChangePassword { get; set; }
}

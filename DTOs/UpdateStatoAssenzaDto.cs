namespace Gestionale.Api.DTOs;

public class UpdateStatoAssenzaDto
{
    public string Stato { get; set; } = string.Empty;
    public string? Note { get; set; }
}

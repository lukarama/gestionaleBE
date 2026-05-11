namespace Gestionale.Api.DTOs;

public class UpdateRequestStatusDto
{
    public string Stato { get; set; } = string.Empty;
    public string? Nota { get; set; }
}

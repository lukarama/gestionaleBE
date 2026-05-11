namespace Gestionale.Api.DTOs;

public class UpdateUtenteVisibilityDto
{
    public string[] Roles { get; set; } = [];

    public string[] Visibility { get; set; } = [];
}

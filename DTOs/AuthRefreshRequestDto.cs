using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs;

public class AuthRefreshRequestDto
{
    [Required]
    [MinLength(1)]
    public string RefreshToken { get; set; } = string.Empty;
}

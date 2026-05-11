using System.ComponentModel.DataAnnotations;

namespace Gestionale.Api.DTOs;

public class AuthLoginRequestDto
{
    [Required]
    [MinLength(1)]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Password { get; set; } = string.Empty;
}

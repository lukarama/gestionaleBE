using System;
using System.Collections.Generic;

namespace Gestionale.Api.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public CurrentUserDto User { get; set; } = new();
}

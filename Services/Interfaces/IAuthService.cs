using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> LoginAsync(AuthLoginRequestDto request, string? ipAddress, string? userAgent);

    Task<ServiceResult<AuthResponseDto>> RefreshAsync(AuthRefreshRequestDto request, string? ipAddress, string? userAgent);

    Task<ServiceResult<bool>> LogoutAsync(string refreshToken);

    Task<AuthUserProfileDto?> GetCurrentUserAsync(int userId);
}

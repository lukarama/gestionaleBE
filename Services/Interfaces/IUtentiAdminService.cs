using Gestionale.Api.Common;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IUtentiAdminService
{
    Task<List<UtenteDipendenteAdminDto>> GetDipendentiAccountsAsync();

    Task<ServiceResult<UtenteDipendenteAdminDto>> CreateDipendenteAccountAsync(CreateUtenteDipendenteDto dto);

    Task<ServiceResult<UtenteDipendenteAdminDto>> UpdateVisibilityAsync(int userId, UpdateUtenteVisibilityDto dto);

    Task<ServiceResult<ResetUtentePasswordResultDto>> ResetPasswordAsync(int userId, ResetUtentePasswordDto? dto = null);

    Task<ServiceResult<bool>> DeleteAccountAsync(int userId);
}

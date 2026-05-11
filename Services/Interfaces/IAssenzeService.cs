using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IAssenzeService
{
    Task<List<TipoAssenzaDto>> GetTipiAssenzaAsync();
    Task<List<DipendenteSelectDto>> GetDipendentiSelectAsync();
    Task<ServiceResult<List<AssenzaListDto>>> GetMieRichiesteAsync(UserContext user);
    Task<ServiceResult<AssenzaListDto>> CreateRichiestaAsync(CreateAssenzaDto dto, UserContext user);
    Task<ServiceResult<AssenzaListDto>> UpdateStatoRichiestaAsync(int id, UpdateStatoAssenzaDto dto, UserContext user);
    Task<ServiceResult<bool>> DeleteRichiestaAsync(int id, UserContext user);
}

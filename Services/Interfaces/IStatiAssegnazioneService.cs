using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IStatiAssegnazioneService
{
    Task<List<StatoAssegnazioneListDto>> GetAllAsync();
    Task<List<SelectOptionDto>> GetAttiviAsync();
}

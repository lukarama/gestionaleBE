using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;
using Gestionale.Api.Security;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DipendentiController : ControllerBase
    {
        private readonly IDipendentiService _dipendentiService;
        private readonly IAssenzeService _assenzeService;
        private readonly ICurrentUserService _currentUserService;

        public DipendentiController(
            IDipendentiService dipendentiService,
            IAssenzeService assenzeService,
            ICurrentUserService currentUserService)
        {
            _dipendentiService = dipendentiService;
            _assenzeService = assenzeService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [HasPermission(PermissionCodes.DipendentiReadAll)]
        public async Task<ActionResult<IEnumerable<DipendenteListDto>>> GetAll()
        {
            var dipendenti = await _dipendentiService.GetAllAsync();
            return Ok(dipendenti);
        }

        [HttpGet("me")]
        [HasPermission(PermissionCodes.DipendentiReadSelf)]
        public async Task<ActionResult<DipendenteDetailDto>> GetCurrentDipendente()
        {
            if (!_currentUserService.DipendenteId.HasValue)
            {
                return Forbid();
            }

            var dipendente = await _dipendentiService.GetByIdAsync(_currentUserService.DipendenteId.Value);
            if (dipendente == null)
            {
                return NotFound(new { message = "Dipendente non trovato" });
            }

            return Ok(dipendente);
        }

        [HttpGet("select")]
        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<ActionResult<List<DipendenteSelectDto>>> GetDipendentiSelect()
        {
            var result = await _assenzeService.GetDipendentiSelectAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DipendenteDetailDto>> GetById(int id)
        {
            if (!CanReadDipendente(id))
            {
                return Forbid();
            }

            var dipendente = await _dipendentiService.GetByIdAsync(id);

            if (dipendente == null)
            {
                return NotFound(new { message = "Dipendente non trovato" });
            }

            return Ok(dipendente);
        }

        [HttpGet("{id}/scheda")]
        public async Task<ActionResult<DipendenteSchedaDto>> GetScheda(int id)
        {
            if (!CanReadDipendente(id))
            {
                return Forbid();
            }

            var dipendente = await _dipendentiService.GetSchedaAsync(id);

            if (dipendente == null)
            {
                return NotFound(new { message = "Dipendente non trovato" });
            }

            return Ok(dipendente);
        }

        [HttpPost]
        [HasPermission(PermissionCodes.DipendentiCreate)]
        public async Task<ActionResult<DipendenteDetailDto>> Create(CreateDipendenteDto dto)
        {
            var result = await _dipendentiService.CreateAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [HasPermission(PermissionCodes.DipendentiUpdateAll)]
        public async Task<IActionResult> Update(int id, UpdateDipendenteDto dto)
        {
            var result = await _dipendentiService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message == "Dipendente non trovato.")
                {
                    return NotFound(new { message = result.Message });
                }

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [HasPermission(PermissionCodes.DipendentiDelete)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dipendentiService.DeleteAsync(id);

            if (!result.Success)
            {
                if (result.Message == "Dipendente non trovato.")
                {
                    return NotFound(new { message = result.Message });
                }

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        private bool CanReadDipendente(int dipendenteId)
        {
            return _currentUserService.HasPermission(PermissionCodes.DipendentiReadAll) ||
                   (_currentUserService.HasPermission(PermissionCodes.DipendentiReadSelf) &&
                    _currentUserService.DipendenteId == dipendenteId);
        }
    }
}

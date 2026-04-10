using Gestionale.Api.DTOs;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestionale.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MezziController : ControllerBase
    {
        private readonly IMezziService _mezziService;

        public MezziController(IMezziService mezziService)
        {
            _mezziService = mezziService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MezzoListDto>>> GetAll()
        {
            var mezzi = await _mezziService.GetAllAsync();
            return Ok(mezzi);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MezzoDetailDto>> GetById(int id)
        {
            var mezzo = await _mezziService.GetByIdAsync(id);

            if (mezzo == null)
                return NotFound(new { message = "Mezzo non trovato" });

            return Ok(mezzo);
        }

        [HttpGet("dashboard/scadenze-native")]
        public async Task<ActionResult<List<ScadenzaMezzoDashboardDto>>> GetDashboardScadenzeNative([FromQuery] int giorni = 30)
        {
            var result = await _mezziService.GetDashboardScadenzeNativeAsync(giorni);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MezzoDetailDto>> Create(CreateMezzoDto dto)
        {
            var result = await _mezziService.CreateAsync(dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateMezzoDto dto)
        {
            var result = await _mezziService.UpdateAsync(id, dto);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mezziService.DeleteAsync(id);

            if (!result.Success)
                return StatusCode(result.StatusCode ?? 400, new { message = result.Message });

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Terapeuta;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Enums;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TerapeutaController : ControllerBase
    {
        private readonly ITerapeutaService _terapeutaService;

        public TerapeutaController(ITerapeutaService terapeutaService)
        {
            _terapeutaService = terapeutaService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetAll()
        {
            var terapeutas = await _terapeutaService.GetAllAsync();
            return Ok(terapeutas);
        }

        [HttpGet("activos")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetActivos()
        {
            var terapeutas = await _terapeutaService.GetActivosAsync();
            return Ok(terapeutas);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetById(int id)
        {
            var terapeuta = await _terapeutaService.GetByIdAsync(id);
            if (terapeuta == null)
                return NotFound();

            return Ok(terapeuta);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetByUserId(string userId)
        {
            var terapeuta = await _terapeutaService.GetByUserIdAsync(userId);
            if (terapeuta == null)
                return NotFound();

            if (User.FindFirst("sub")?.Value != userId &&
                !User.IsInRole(UserRoles.SUPER_ADMIN) &&
                !User.IsInRole(UserRoles.ADMIN))
                return Forbid();

            return Ok(terapeuta);
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> Create([FromBody] CreateTerapeutaDto dto)
        {
            try
            {
                var terapeuta = await _terapeutaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = terapeuta.Id }, terapeuta);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateTerapeutaDto dto)
        {
            try
            {
                var terapeuta = await _terapeutaService.UpdateAsync(id, dto);
                if (terapeuta == null)
                    return NotFound();

                return Ok(terapeuta);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/estado")]
        [Authorize(Roles = UserRoles.SUPER_ADMIN)]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] CambioEstadoDto cambioEstado)
        {
            try
            {
                var result = await _terapeutaService.CambiarEstadoAsync(id, cambioEstado);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/tarifas")]
        [Authorize(Roles = UserRoles.SUPER_ADMIN)]
        public async Task<ActionResult> UpdateTarifas(int id, [FromBody] UpdateTarifasRequest tarifas)
        {
            if (tarifas.TarifaBase < 0 || tarifas.TarifaExtra < 0)
                return BadRequest(new { message = "Las tarifas no pueden ser negativas" });

            var result = await _terapeutaService.UpdateTarifasAsync(id, tarifas.TarifaBase, tarifas.TarifaExtra);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }

    public class UpdateTarifasRequest
    {
        public decimal TarifaBase { get; set; }
        public decimal TarifaExtra { get; set; }
    }
}
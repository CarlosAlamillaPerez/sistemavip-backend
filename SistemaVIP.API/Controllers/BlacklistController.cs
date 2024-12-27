using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlacklistController : ControllerBase
    {
        private readonly IBlacklistService _blacklistService;

        public BlacklistController(IBlacklistService blacklistService)
        {
            _blacklistService = blacklistService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetAll()
        {
            var registros = await _blacklistService.GetAllAsync();
            return Ok(registros);
        }

        [HttpGet("activos")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetActivos()
        {
            var registros = await _blacklistService.GetActivosAsync();
            return Ok(registros);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetById(int id)
        {
            var registro = await _blacklistService.GetByIdAsync(id);
            if (registro == null)
                return NotFound();

            return Ok(registro);
        }

        [HttpGet("verificar/telefono/{telefono}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> VerificarTelefono(string telefono)
        {
            var resultado = await _blacklistService.VerificarTelefonoAsync(telefono);
            return Ok(resultado);
        }

        [HttpGet("verificar/email/{email}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> VerificarEmail(string email)
        {
            var resultado = await _blacklistService.VerificarEmailAsync(email);
            return Ok(resultado);
        }

        [HttpGet("existe")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> ExisteRegistroActivo(
            [FromQuery] string telefono,
            [FromQuery] string email)
        {
            var existe = await _blacklistService.ExisteRegistroActivoAsync(telefono, email);
            return Ok(new { existe });
        }

        [HttpGet("filtro")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetByFiltro([FromQuery] BlacklistFiltroDto filtro)
        {
            var registros = await _blacklistService.GetByFiltroAsync(filtro);
            return Ok(registros);
        }

        [HttpGet("ultimos/{cantidad}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetUltimosRegistros(int cantidad = 10)
        {
            var registros = await _blacklistService.GetUltimosRegistrosAsync(cantidad);
            return Ok(registros);
        }

        [HttpGet("estadisticas")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetEstadisticas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var estadisticas = await _blacklistService.GetEstadisticasRegistrosAsync(fechaInicio, fechaFin);
            return Ok(estadisticas);
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> Create([FromBody] CreateBlacklistDto dto)
        {
            try
            {
                var registro = await _blacklistService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = registro.Id }, registro);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateBlacklistDto dto)
        {
            try
            {
                var registro = await _blacklistService.UpdateAsync(id, dto);
                if (registro == null)
                    return NotFound();

                return Ok(registro);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _blacklistService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/estado")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> CambiarEstado(
            int id,
            [FromBody] CambioEstadoDto cambioEstado)
        {
            try
            {
                var result = await _blacklistService.CambiarEstadoAsync(
                    id,
                    cambioEstado.Estado,
                    cambioEstado.MotivoEstado);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
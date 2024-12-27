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
    [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
    public class BitacoraController : ControllerBase
    {
        private readonly IBitacoraService _bitacoraService;

        public BitacoraController(IBitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

        [HttpGet("registro/{tabla}/{idRegistro}")]
        public async Task<ActionResult> GetByRegistro(string tabla, string idRegistro)
        {
            try
            {
                var registros = await _bitacoraService.GetByRegistroAsync(tabla, idRegistro);
                return Ok(registros);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("filtro")]
        public async Task<ActionResult> GetByFiltro(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] string tabla = null,
            [FromQuery] string idUsuario = null)
        {
            try
            {
                // Validar rango de fechas
                if (fechaFin < fechaInicio)
                {
                    return BadRequest(new { message = "La fecha final debe ser mayor o igual a la fecha inicial" });
                }

                // Limitar la consulta a un máximo de 31 días
                var maxDias = 31;
                var diferenciaDias = (fechaFin - fechaInicio).Days;
                if (diferenciaDias > maxDias)
                {
                    return BadRequest(new { message = $"El rango de fechas no puede exceder {maxDias} días" });
                }

                var registros = await _bitacoraService.GetByFiltroAsync(
                    fechaInicio,
                    fechaFin,
                    tabla,
                    idUsuario);

                return Ok(registros);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("tablas")]
        public ActionResult GetTablasMonitoreo()
        {
            return Ok(BitacoraEnum.TablasMonitoreo);
        }

        [HttpGet("acciones")]
        public ActionResult GetTiposAccion()
        {
            return Ok(BitacoraEnum.TiposAccion);
        }
    }
}
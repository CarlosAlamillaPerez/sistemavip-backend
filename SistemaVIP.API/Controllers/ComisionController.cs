using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComisionController : ControllerBase
    {
        private readonly IComisionService _comisionService;

        public ComisionController(IComisionService comisionService)
        {
            _comisionService = comisionService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetAll()
        {
            var comisiones = await _comisionService.GetAllAsync();
            return Ok(comisiones);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetById(int id)
        {
            var comision = await _comisionService.GetByIdAsync(id);
            if (comision == null)
                return NotFound();

            return Ok(comision);
        }

        [HttpGet("filtro")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetByFiltro([FromQuery] FiltroComisionesDto filtro)
        {
            var comisiones = await _comisionService.GetByFiltroAsync(filtro);
            return Ok(comisiones);
        }

        [HttpGet("resumen-presentadores")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetResumenPresentadores()
        {
            var resumen = await _comisionService.GetResumenPresentadoresAsync();
            return Ok(resumen);
        }

        [HttpGet("resumen-presentador/{presentadorId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetResumenPresentador(int presentadorId)
        {
            var resumen = await _comisionService.GetResumenPresentadorAsync(presentadorId);
            return Ok(resumen);
        }

        [HttpPost("liquidar-pago/{comisionId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> RegistrarPago(int comisionId, [FromBody] RegistroPagoComisionDto dto)
        {
            try
            {
                var comision = await _comisionService.RegistrarPagoTerapeutaAsync(comisionId, dto);
                return Ok(comision);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{comisionId}/estado")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult<ComisionDto>> CambiarEstadoPago(int comisionId, [FromBody] CambioEstadoComisionDto dto)
        {
            try
            {
                var comision = await _comisionService.CambiarEstadoPagoAsync(comisionId, dto.Estado, dto.Notas);
                return Ok(comision);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("pendientes-liquidacion/{presentadorId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetComisionesPendientesLiquidacion(int presentadorId)
        {
            var comisiones = await _comisionService.GetComisionesPendientesLiquidacionAsync(presentadorId);
            return Ok(comisiones);
        }

        [HttpGet("liquidadas")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetComisionesLiquidadas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var comisiones = await _comisionService.GetComisionesLiquidadasAsync(fechaInicio, fechaFin);
            return Ok(comisiones);
        }
    }
}
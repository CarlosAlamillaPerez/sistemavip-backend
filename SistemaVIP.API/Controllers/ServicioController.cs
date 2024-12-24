using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Servicio;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicioController : ControllerBase
    {
        private readonly IServicioService _servicioService;

        public ServicioController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<List<ServicioDto>>> GetAll()
        {
            var servicios = await _servicioService.GetAllAsync();
            return Ok(servicios);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<ServicioDto>> GetById(int id)
        {
            var servicio = await _servicioService.GetByIdAsync(id);
            if (servicio == null)
                return NotFound();

            return Ok(servicio);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<ServicioDto>> Create(CreateServicioDto createDto)
        {
            try
            {
                var servicio = await _servicioService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = servicio.Id }, servicio);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<ServicioDto>> Update(int id, UpdateServicioDto updateDto)
        {
            var servicio = await _servicioService.UpdateAsync(id, updateDto);
            if (servicio == null)
                return NotFound();

            return Ok(servicio);
        }

        [HttpPost("{id}/cancelar")]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<ServicioDto>> Cancelar(int id, CancelacionServicioDto cancelacionDto)
        {
            var userId = User.FindFirst("sub")?.Value;
            var servicio = await _servicioService.CancelarServicioAsync(id, cancelacionDto, userId);
            if (servicio == null)
                return NotFound();

            return Ok(servicio);
        }

        [HttpGet("confirmar/{linkConfirmacion}")]
        [AllowAnonymous]
        public async Task<ActionResult<ServicioTerapeutaDto>> GetByLinkConfirmacion(Guid linkConfirmacion)
        {
            var servicioTerapeuta = await _servicioService.GetServicioTerapeutaByLinkConfirmacionAsync(linkConfirmacion);
            if (servicioTerapeuta == null)
                return NotFound();

            return Ok(servicioTerapeuta);
        }

        [HttpPost("confirmar")]
        [AllowAnonymous]
        public async Task<ActionResult<ServicioTerapeutaDto>> ConfirmarInicio(ConfirmacionServicioDto confirmacionDto)
        {
            var servicioTerapeuta = await _servicioService.ConfirmarInicioServicioAsync(confirmacionDto);
            if (servicioTerapeuta == null)
                return NotFound("Link de confirmación inválido o expirado");

            return Ok(servicioTerapeuta);
        }

        [HttpGet("finalizar/{linkFinalizacion}")]
        [AllowAnonymous]
        public async Task<ActionResult<ServicioTerapeutaDto>> GetByLinkFinalizacion(Guid linkFinalizacion)
        {
            var servicioTerapeuta = await _servicioService.GetServicioTerapeutaByLinkFinalizacionAsync(linkFinalizacion);
            if (servicioTerapeuta == null)
                return NotFound();

            return Ok(servicioTerapeuta);
        }

        [HttpPost("finalizar")]
        [AllowAnonymous]
        public async Task<ActionResult<ServicioTerapeutaDto>> FinalizarServicio(FinalizacionServicioDto finalizacionDto)
        {
            var servicioTerapeuta = await _servicioService.FinalizarServicioAsync(finalizacionDto);
            if (servicioTerapeuta == null)
                return NotFound("Link de finalización inválido o expirado");

            return Ok(servicioTerapeuta);
        }

        [HttpGet("presentador/{presentadorId}")]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<List<ServicioDto>>> GetByPresentador(int presentadorId)
        {
            var servicios = await _servicioService.GetServiciosByPresentadorAsync(presentadorId);
            return Ok(servicios);
        }

        [HttpGet("terapeuta/{terapeutaId}")]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<List<ServicioDto>>> GetByTerapeuta(int terapeutaId)
        {
            var servicios = await _servicioService.GetServiciosByTerapeutaAsync(terapeutaId);
            return Ok(servicios);
        }

        [HttpGet("fecha/{fecha}")]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<List<ServicioDto>>> GetByFecha(DateTime fecha)
        {
            var servicios = await _servicioService.GetServiciosByFechaAsync(fecha);
            return Ok(servicios);
        }

        [HttpGet("activos")]
        [Authorize(Roles = "Admin,SuperAdmin,Presentador")]
        public async Task<ActionResult<List<ServicioDto>>> GetActivos()
        {
            var servicios = await _servicioService.GetServiciosActivosAsync();
            return Ok(servicios);
        }

        [HttpPost("{servicioTerapeutaId}/comprobantes")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<ServicioTerapeutaDto>> AgregarComprobantePago(
            int servicioTerapeutaId,
            [FromBody] CreateComprobantePagoDto dto)
        {
            try
            {
                var servicio = await _servicioService.AgregarComprobantePagoAsync(servicioTerapeutaId, dto);
                return Ok(servicio);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPatch("{servicioTerapeutaId}/comprobantes/{comprobanteId}/estado")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult<ServicioTerapeutaDto>> ActualizarEstadoComprobante(
            int servicioTerapeutaId,
            int comprobanteId,
            [FromBody] UpdateComprobanteEstadoDto dto)
        {
            try
            {
                var servicio = await _servicioService.ActualizarEstadoComprobanteAsync(
                    servicioTerapeutaId,
                    comprobanteId,
                    dto);
                return Ok(servicio);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
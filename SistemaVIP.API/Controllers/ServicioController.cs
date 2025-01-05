using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IServicioExtraService _servicioExtraService;

        public ServicioController(IServicioService servicioService, IServicioExtraService servicioExtraService)
        {
            _servicioService = servicioService;
            _servicioExtraService = servicioExtraService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult<List<ServicioDto>>> GetAll()
        {
            var servicios = await _servicioService.GetAllAsync();
            return Ok(servicios);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<ServicioDto>> GetById(int id)
        {
            var servicio = await _servicioService.GetByIdAsync(id);
            if (servicio == null)
                return NotFound();

            return Ok(servicio);
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
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
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<ServicioDto>> Update(int id, UpdateServicioDto updateDto)
        {
            var servicio = await _servicioService.UpdateAsync(id, updateDto);
            if (servicio == null)
                return NotFound();

            return Ok(servicio);
        }

        [HttpPost("{id}/cancelar")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
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
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<List<ServicioDto>>> GetByPresentador(int presentadorId)
        {
            var servicios = await _servicioService.GetServiciosByPresentadorAsync(presentadorId);
            return Ok(servicios);
        }

        [HttpGet("terapeuta/{terapeutaId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<List<ServicioDto>>> GetByTerapeuta(int terapeutaId)
        {
            var servicios = await _servicioService.GetServiciosByTerapeutaAsync(terapeutaId);
            return Ok(servicios);
        }

        [HttpGet("fecha/{fecha}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<List<ServicioDto>>> GetByFecha(DateTime fecha)
        {
            var servicios = await _servicioService.GetServiciosByFechaAsync(fecha);
            return Ok(servicios);
        }

        [HttpGet("activos")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<List<ServicioDto>>> GetActivos()
        {
            var servicios = await _servicioService.GetServiciosActivosAsync();
            return Ok(servicios);
        }

        [HttpGet("{servicioTerapeutaId}/conciliacion")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetConciliacion(int servicioTerapeutaId)
        {
            try
            {
                var conciliacion = await _servicioService.GetConciliacionServicioAsync(servicioTerapeutaId);
                return Ok(conciliacion);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{servicioTerapeutaId}/conciliacion")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> RealizarConciliacion(int servicioTerapeutaId)
        {
            try
            {
                var resultado = await _servicioService.RealizarConciliacionAsync(servicioTerapeutaId);

                if (resultado.RequiereRevision)
                {
                    return BadRequest(new
                    {
                        message = "La conciliación requiere revisión",
                        detalles = resultado
                    });
                }

                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{servicioTerapeutaId}/conciliacion/validar")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> ValidarConciliacion(int servicioTerapeutaId)
        {
            try
            {
                var esValida = await _servicioService.ValidarConciliacionAsync(servicioTerapeutaId);
                return Ok(new { esValida });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("notificar-cancelaciones")]
        [AllowAnonymous] // Solo para llamadas internas
        public async Task<ActionResult> NotificarCancelacionesSemanales()
        {
            try
            {
                await _servicioService.NotificarCancelacionesExcesivasAsync();
                return Ok(new { message = "Notificaciones enviadas correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al enviar notificaciones", error = ex.Message });
            }
        }

        [HttpGet("servicios-extra/catalogo")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<List<ServicioExtraCatalogoDto>>> GetServiciosExtraCatalogo()
        {
            try
            {
                var catalogo = await _servicioExtraService.GetCatalogoActivoAsync();
                return Ok(catalogo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el catálogo de servicios extra", error = ex.Message });
            }
        }

        [HttpPost("{servicioTerapeutaId}/servicios-extra")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> AgregarServiciosExtra(int servicioTerapeutaId, [FromBody] CreateServicioExtraDto dto)
        {
            try
            {
                await _servicioExtraService.AgregarServiciosExtraAsync(servicioTerapeutaId, dto);
                return Ok(new { message = "Servicios extra agregados correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al agregar servicios extra", error = ex.Message });
            }
        }

        [HttpGet("{servicioTerapeutaId}/servicios-extra")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<List<ServicioExtraDetalleDto>>> GetServiciosExtra(int servicioTerapeutaId)
        {
            try
            {
                var serviciosExtra = await _servicioExtraService.GetServiciosExtraByServicioAsync(servicioTerapeutaId);
                return Ok(serviciosExtra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los servicios extra", error = ex.Message });
            }
        }

        [HttpPut("{servicioTerapeutaId}/servicios-extra/{servicioExtraId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<ServicioExtraDetalleDto>> UpdateServicioExtra(int servicioTerapeutaId,int servicioExtraId,[FromBody] UpdateServicioExtraDto dto)
        {
            try
            {
                var resultado = await _servicioExtraService.UpdateServicioExtraAsync(servicioTerapeutaId, servicioExtraId, dto);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el servicio extra", error = ex.Message });
            }
        }

        [HttpDelete("{servicioTerapeutaId}/servicios-extra/{servicioExtraId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> DeleteServicioExtra(int servicioTerapeutaId, int servicioExtraId)
        {
            try
            {
                await _servicioExtraService.DeleteServicioExtraAsync(servicioTerapeutaId, servicioExtraId);
                return Ok(new { message = "Servicio extra eliminado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el servicio extra", error = ex.Message });
            }
        }

        [HttpGet("{servicioTerapeutaId}/comprobantes")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<List<ComprobantePagoDto>>> GetComprobantesPago(int servicioTerapeutaId)
        {
            try
            {
                var comprobantes = await _servicioService.GetComprobantesPagoByServicioAsync(servicioTerapeutaId);
                return Ok(comprobantes);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{servicioTerapeutaId}/comprobantes")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<ServicioTerapeutaDto>> AgregarComprobantePago(int servicioTerapeutaId,[FromBody] CreateComprobantePagoDto dto)
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
            catch (Exception ex)
            {
                // Log del error
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("{servicioTerapeutaId}/comprobantes/multiple")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult<ServicioTerapeutaDto>> AgregarComprobantesMultiples(int servicioTerapeutaId,[FromBody] CreateComprobantesMultiplesDto dto)
        {
            try
            {
                var servicio = await _servicioService.AgregarComprobantesMultiplesAsync(servicioTerapeutaId, dto);
                return Ok(servicio);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{servicioTerapeutaId}/comprobantes/{comprobanteId}/estado")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult<ServicioTerapeutaDto>> ActualizarEstadoComprobante(int servicioTerapeutaId,int comprobanteId,[FromBody] UpdateComprobanteEstadoDto dto)
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

        [HttpDelete("{servicioTerapeutaId}/comprobantes/{comprobanteId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> DeleteComprobante(int servicioTerapeutaId, int comprobanteId)
        {
            try
            {
                await _servicioService.EliminarComprobantePagoAsync(servicioTerapeutaId, comprobanteId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _servicioService.DeleteServicioAsync(id);
                if (!result)
                    return NotFound(new { message = "Servicio no encontrado o no puede ser eliminado" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
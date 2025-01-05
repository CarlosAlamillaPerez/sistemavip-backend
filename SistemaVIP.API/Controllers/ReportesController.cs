// ReportesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs.Reportes;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Infrastructure.Persistence.Context;
using SistemaVIP.Infrastructure.Services;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IReportesService _reportesService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _context;

        public ReportesController(
            IReportesService reportesService,
            ICurrentUserService currentUserService,
            ApplicationDbContext context)
        {
            _reportesService = reportesService;
            _currentUserService = currentUserService;
            _context = context;
        }

        [HttpGet("presentadores")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetReportePresentadores([FromQuery] ReportesPresentadorFiltroDto filtro)
        {
            try
            {
                var reporte = await _reportesService.GetReportePresentadoresAsync(filtro);
                return Ok(reporte);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("presentadores/{presentadorId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetReportePresentador(int presentadorId,[FromQuery] DateTime fechaInicio,[FromQuery] DateTime fechaFin)
        {
            try
            {
                // Si es presentador, validar que solo acceda a sus propios reportes
                if (User.IsInRole(UserRoles.PRESENTADOR))
                {
                    var userId = _currentUserService.GetUserId();
                    if (string.IsNullOrEmpty(userId))
                        return Unauthorized(new { message = "Usuario no identificado" });

                    // Obtener el presentador asociado al usuario actual
                    var presentadorActual = await _context.Presentadores
                        .FirstOrDefaultAsync(p => p.UserId == userId);

                    if (presentadorActual == null)
                        return Unauthorized(new { message = "Usuario no asociado a un presentador" });

                    // Validar que el presentadorId corresponda al usuario actual
                    if (presentadorActual.Id != presentadorId)
                        return Forbid();
                }

                var reporte = await _reportesService.GetReportePresentadorAsync(presentadorId, fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("terapeutas")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetReporteTerapeutas([FromQuery] ReporteTerapeutaFiltroDto filtro)
        {
            try
            {
                var reporte = await _reportesService.GetReporteTerapeutasAsync(filtro);
                return Ok(reporte);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("terapeutas/{terapeutaId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetReporteTerapeuta(int terapeutaId,[FromQuery] DateTime fechaInicio,[FromQuery] DateTime fechaFin)
        {
            try
            {
                var reporte = await _reportesService.GetReporteTerapeutaAsync(terapeutaId, fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("servicios")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetReporteServicios([FromQuery] DateTime fechaInicio,[FromQuery] DateTime fechaFin)
        {
            try
            {
                var reporte = await _reportesService.GetReporteServiciosAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
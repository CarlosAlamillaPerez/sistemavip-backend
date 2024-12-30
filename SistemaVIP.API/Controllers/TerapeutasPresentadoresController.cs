using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs.TerapeutaPresentador;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Enums;
using System.Threading.Tasks;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TerapeutasPresentadoresController : ControllerBase
    {
        private readonly ITerapeutasPresentadoresService _service;

        public TerapeutasPresentadoresController(ITerapeutasPresentadoresService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetAll()
        {
            var asignaciones = await _service.GetAllAsync();
            return Ok(asignaciones);
        }

        [HttpGet("presentador/{presentadorId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}, {UserRoles.PRESENTADOR}")]
        public async Task<ActionResult> GetTerapeutasByPresentador(int presentadorId)
        {
            // Si es presentador, verificar que solo acceda a sus propias asignaciones
            if (User.IsInRole(UserRoles.PRESENTADOR))
            {
                // Aquí deberías verificar que el presentadorId corresponde al usuario actual
                // Implementar lógica según necesidades
            }

            var terapeutas = await _service.GetTerapeutasByPresentadorIdAsync(presentadorId);
            return Ok(terapeutas);
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> AsignarTerapeutaPresentador([FromBody] AsignarTerapeutaPresentadorDto dto)
        {
            try
            {
                var asignacion = await _service.AsignarTerapeutaPresentadorAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { }, asignacion);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{terapeutaId}/{presentadorId}/estado")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> UpdateEstado(int terapeutaId, int presentadorId, [FromBody] string estado)
        {
            try
            {
                var result = await _service.UpdateEstadoAsync(terapeutaId, presentadorId, estado);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{terapeutaId}/{presentadorId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> EliminarAsignacion(int terapeutaId, int presentadorId)
        {
            try
            {
                var result = await _service.EliminarAsignacionAsync(terapeutaId, presentadorId);
                if (!result)
                    return NotFound(new { message = "No se encontró la asignación activa especificada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
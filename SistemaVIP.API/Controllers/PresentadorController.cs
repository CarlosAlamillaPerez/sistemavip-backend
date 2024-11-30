﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs.Presentador;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Enums;
using System.Threading.Tasks;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PresentadorController : ControllerBase
    {
        private readonly IPresentadorService _presentadorService;

        public PresentadorController(IPresentadorService presentadorService)
        {
            _presentadorService = presentadorService;
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetAll()
        {
            var presentadores = await _presentadorService.GetAllAsync();
            return Ok(presentadores);
        }

        [HttpGet("activos")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetActivos()
        {
            var presentadores = await _presentadorService.GetActivosAsync();
            return Ok(presentadores);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetById(int id)
        {
            var presentador = await _presentadorService.GetByIdAsync(id);
            if (presentador == null)
                return NotFound();

            return Ok(presentador);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetByUserId(string userId)
        {
            var presentador = await _presentadorService.GetByUserIdAsync(userId);
            if (presentador == null)
                return NotFound();

            // Verificar que el usuario actual solo pueda ver su propio perfil
            if (User.FindFirst("sub")?.Value != userId && !User.IsInRole(UserRoles.SUPER_ADMIN) && !User.IsInRole(UserRoles.ADMIN))
                return Forbid();

            return Ok(presentador);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.SUPER_ADMIN)]
        public async Task<ActionResult> Create([FromBody] CreatePresentadorDto dto)
        {
            try
            {
                var presentador = await _presentadorService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = presentador.Id }, presentador);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdatePresentadorDto dto)
        {
            try
            {
                var presentador = await _presentadorService.UpdateAsync(id, dto);
                if (presentador == null)
                    return NotFound();

                return Ok(presentador);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.SUPER_ADMIN)]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _presentadorService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/estado")]
        [Authorize(Roles = UserRoles.SUPER_ADMIN)]
        public async Task<ActionResult> UpdateEstado(int id, [FromBody] string estado)
        {
            var result = await _presentadorService.UpdateEstadoAsync(id, estado);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/comision")]
        [Authorize(Roles = UserRoles.SUPER_ADMIN)]
        public async Task<ActionResult> UpdateComision(int id, [FromBody] decimal nuevoPorcentaje)
        {
            if (nuevoPorcentaje < 0 || nuevoPorcentaje > 100)
                return BadRequest(new { message = "El porcentaje debe estar entre 0 y 100" });

            var result = await _presentadorService.UpdateComisionAsync(id, nuevoPorcentaje);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
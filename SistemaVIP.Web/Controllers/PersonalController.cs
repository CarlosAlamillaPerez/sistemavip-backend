﻿using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Presentador;
using SistemaVIP.Core.DTOs.Terapeuta;
using SistemaVIP.Core.DTOs.TerapeutaPresentador;
using SistemaVIP.Web.Attributes;
using SistemaVIP.Web.Interfaces;
using System.Security.Claims;

namespace SistemaVIP.Web.Controllers
{
    [CustomAuthorization("SUPER_ADMIN", "ADMIN")]
    public class PersonalController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<HomeController> _logger;


        public PersonalController(IApiService apiService, ILogger<HomeController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Presentadores()
        {
            try
            {
                var presentadores = await _apiService.GetAsync<List<PresentadorDto>>("api/Presentador");
                return View("~/Views/Personal/Presentadores/Index.cshtml", presentadores);
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente
                return View("~/Views/Personal/Presentadores/Index.cshtml", new List<PresentadorDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Terapeutas()
        {
            try
            {
                var terapeutas = await _apiService.GetAsync<List<TerapeutaDto>>("api/Terapeuta");
                return View("~/Views/Personal/Terapeutas/Index.cshtml", terapeutas);
            }
            catch (Exception ex)
            {
                // Si hay un error, devolvemos una lista vacía para evitar el null
                return View("~/Views/Personal/Terapeutas/Index.cshtml", new List<TerapeutaDto>());
            }
        }


        [HttpGet]
        public async Task<IActionResult> Asignaciones()
        {
            try
            {
                return View("~/Views/Personal/Asignaciones/Index.cshtml");
            }
            catch (Exception ex)
            {
                return View("~/Views/Personal/Asignaciones/Index.cshtml");
            }
        }


        #region Presentadores
        [HttpGet]
        public async Task<IActionResult> ObtenerPresentadores()
        {
            try
            {
                // Verificar autenticación y rol
                var isAuthenticated = User.Identity.IsAuthenticated;
                var roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Log para debugging
                _logger.LogInformation($"IsAuthenticated: {isAuthenticated}");
                _logger.LogInformation($"Roles: {string.Join(", ", roles)}");
                _logger.LogInformation($"UserId: {userId}");

                var presentadores = await _apiService.GetAsync<List<PresentadorDto>>("api/Presentador");
                return Json(new { success = true, data = presentadores });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFormularioPresentador(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/{id}");

                    // Convertir PresentadorDto a CreatePresentadorDto
                    var createDto = new CreatePresentadorDto
                    {
                        Nombre = presentador.Nombre,
                        Apellido = presentador.Apellido,
                        Telefono = presentador.Telefono,
                        Email = presentador.Email,
                        DocumentoIdentidad = presentador.DocumentoIdentidad,
                        PorcentajeComision = presentador.PorcentajeComision,
                        FotoUrl = presentador.FotoUrl,
                        Notas = presentador.Notas
                    };

                    // Pasar el ID a la vista mediante ViewBag para saber que es una edición
                    ViewBag.PresentadorId = id;
                    return PartialView("~/Views/Personal/Presentadores/_Modal_FormPresentador.cshtml", createDto);
                }

                return PartialView("~/Views/Personal/Presentadores/_Modal_FormPresentador.cshtml", new CreatePresentadorDto());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetallePresentador(int id)
        {
            try
            {
                // Obtener información del presentador
                var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/{id}");

                // Obtener terapeutas asignados
                var terapeutasAsignados = await _apiService.GetAsync<List<TerapeutasPorPresentadorDto>>($"api/TerapeutasPresentadores/presentador/{id}");

                var detalleCompleto = new PresentadorDetalleDto
                {
                    Presentador = presentador,
                    TerapeutasAsignados = terapeutasAsignados
                };

                return PartialView("~/Views/Personal/Presentadores/_Modal_DetallePresentador.cshtml", detalleCompleto);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPresentador([FromBody] CreatePresentadorDto model)
        {
            try
            {
                var result = await _apiService.PostAsync<PresentadorDto>("api/Presentador", model);
                return Json(new { success = true, message = "Presentador guardado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarPresentador(int id, [FromBody] UpdatePresentadorDto model)
        {
            try
            {
                var result = await _apiService.PutAsync<PresentadorDto>($"api/Presentador/{id}", model);
                return Json(new { success = true, message = "Presentador actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTerapeutasPorPresentador(int presentadorId)
        {
            try
            {
                var terapeutas = await _apiService.GetAsync<List<TerapeutasPorPresentadorDto>>($"api/TerapeutasPresentadores/presentador/{presentadorId}");
                return Json(new { success = true, data = terapeutas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> CambiarEstadoPresentador(int id, string estado, string motivo)
        {
            try
            {
                var dto = new CambioEstadoDto { Estado = estado, MotivoEstado = motivo };
                var result = await _apiService.PatchAsync<bool>($"api/Presentador/{id}/estado", dto);
                return Json(new { success = true, message = "Estado actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Terapeutas

        [HttpGet]
        public async Task<IActionResult> ObtenerTerapeutas()
        {
            try
            {
                var isAuthenticated = User.Identity.IsAuthenticated;
                var roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var terapeutas = await _apiService.GetAsync<List<TerapeutaDto>>("api/Terapeuta");
                return Json(new { success = true, data = terapeutas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFormularioTerapeuta(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var terapeuta = await _apiService.GetAsync<TerapeutaDto>($"api/Terapeuta/{id}");
                    var createDto = new CreateTerapeutaDto
                    {
                        Nombre = terapeuta.Nombre,
                        Apellido = terapeuta.Apellido,
                        Telefono = terapeuta.Telefono,
                        Email = terapeuta.Email,
                        DocumentoIdentidad = terapeuta.DocumentoIdentidad,
                        Estatura = terapeuta.Estatura,
                        FotoUrl = terapeuta.FotoUrl,
                        FechaNacimiento = terapeuta.FechaNacimiento,
                        Notas = terapeuta.Notas
                    };

                    ViewBag.TerapeutaId = id;
                    return PartialView("~/Views/Personal/Terapeutas/_Modal_FormTerapeuta.cshtml", createDto);
                }

                return PartialView("~/Views/Personal/Terapeutas/_Modal_FormTerapeuta.cshtml", new CreateTerapeutaDto());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleTerapeuta(int id)
        {
            try
            {
                var terapeuta = await _apiService.GetAsync<TerapeutaDto>($"api/Terapeuta/{id}");
                return PartialView("~/Views/Personal/Terapeutas/_Modal_DetalleTerapeuta.cshtml", terapeuta);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuardarTerapeuta([FromBody] CreateTerapeutaDto model)
        {
            try
            {
                // Establecer las tarifas fijas
                model.TarifaBase = 1200m;
                model.TarifaExtra = 1200m;

                var result = await _apiService.PostAsync<TerapeutaDto>("api/Terapeuta", model);
                return Json(new { success = true, message = "Terapeuta guardada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarTerapeuta(int id, [FromBody] UpdateTerapeutaDto model)
        {
            try
            {
                var terapeutaActual = await _apiService.GetAsync<TerapeutaDto>($"api/Terapeuta/{id}");

                var result = await _apiService.PutAsync<TerapeutaDto>($"api/Terapeuta/{id}", model);
                return Json(new { success = true, message = "Terapeuta actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> CambiarEstadoTerapeuta(int id, string estado, string motivo)
        {
            try
            {
                var dto = new CambioEstadoDto { Estado = estado, MotivoEstado = motivo };
                var result = await _apiService.PatchAsync<bool>($"api/Terapeuta/{id}/estado", dto);
                return Json(new { success = true, message = "Estado actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Asignaciones
        [HttpGet]
        public async Task<IActionResult> ObtenerMatrizAsignaciones()
        {
            try
            {
                var asignaciones = await _apiService.GetAsync<List<AsignacionesPresentadorDto>>("api/TerapeutasPresentadores/asignaciones");
                return Json(new { success = true, data = asignaciones });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTerapeutasDisponibles(int presentadorId)
        {
            try
            {
                // 1. Obtener todas las terapeutas activas
                var terapeutasActivas = await _apiService.GetAsync<List<TerapeutaDto>>("api/Terapeuta/activos");

                // 2. Obtener terapeutas ya asignadas al presentador
                var terapeutasAsignadas = await _apiService.GetAsync<List<TerapeutasPorPresentadorDto>>($"api/TerapeutasPresentadores/presentador/{presentadorId}");

                // 3. Filtrar solo las no asignadas
                var terapeutasDisponibles = terapeutasActivas
                    .Where(t => !terapeutasAsignadas.Any(ta => ta.TerapeutaId == t.Id))
                    .ToList();

                return PartialView("~/Views/Personal/Asignaciones/_Modal_FormAsignacion.cshtml", terapeutasDisponibles);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AsignarTerapeuta([FromBody] AsignarTerapeutaPresentadorDto dto)
        {
            try
            {
                var result = await _apiService.PostAsync<TerapeutaPresentadorDto>("api/TerapeutasPresentadores", dto);
                return Json(new { success = true, message = "Terapeuta asignada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> CambiarEstadoAsignacion([FromBody] CambioEstadoAsignacionDto dto)
        {
            // Agregar log para debug
            var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            _logger.LogInformation($"Usuario autenticado como: {User.Identity.Name}");
            _logger.LogInformation($"Roles del usuario: {string.Join(", ", userRoles)}");

            try
            {
                if (string.IsNullOrEmpty(dto.Estado))
                {
                    return Json(new { success = false, message = "El estado no puede estar vacío" });
                }
                var result = await _apiService.PatchAsync<bool>(
                    $"api/TerapeutasPresentadores/{dto.TerapeutaId}/{dto.PresentadorId}/estado",
                    dto.Estado
                );
                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Estado de asignación actualizado a {dto.Estado} exitosamente"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se pudo actualizar el estado de la asignación"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error al actualizar el estado: {ex.Message}"
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarAsignacion(int presentadorId, int terapeutaId)
        {
            try
            {
                await _apiService.DeleteAsync($"api/TerapeutasPresentadores/{terapeutaId}/{presentadorId}");
                return Json(new { success = true, message = "Asignación eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion


    }
}
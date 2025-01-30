using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Presentador;
using SistemaVIP.Core.DTOs.Reportes;
using SistemaVIP.Core.DTOs.Servicio;
using SistemaVIP.Core.DTOs.TerapeutaPresentador;
using SistemaVIP.Web.Attributes;
using SistemaVIP.Web.Interfaces;
using System.Security.Claims;

namespace SistemaVIP.Web.Controllers
{
    [CustomAuthorization("PRESENTADOR")]
    public class PresentadorController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<PresentadorController> _logger;

        public PresentadorController(IApiService apiService, ILogger<PresentadorController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Presentador/Dashboard/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTerapeutas()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/user/{userId}");
                var terapeutas = await _apiService.GetAsync<List<TerapeutasPorPresentadorDto>>($"api/TerapeutasPresentadores/presentador/{presentador.Id}");
                return Json(new { success = true, data = terapeutas });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener terapeutas del presentador");
                return Json(new { success = false, message = "Error al obtener terapeutas" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerModalTerapeutas()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/user/{userId}");
                var terapeutas = await _apiService.GetAsync<List<TerapeutasPorPresentadorDto>>($"api/TerapeutasPresentadores/presentador/{presentador.Id}");

                return PartialView("~/Views/Presentador/Dashboard/_Modal_Terapeutas.cshtml", terapeutas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener modal de terapeutas");
                return Json(new { success = false, message = "Error al cargar terapeutas" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerServicios(string estado = null, int? terapeutaId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Usuario no identificado" });
                }

                var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/user/{userId}");
                var servicios = await _apiService.GetAsync<List<ServicioDto>>($"api/Servicio/presentador/{presentador.Id}");

                // Aplicar filtros
                if (!string.IsNullOrEmpty(estado))
                {
                    servicios = servicios.Where(s => s.Estado == estado).ToList();
                }
                if (terapeutaId.HasValue)
                {
                    servicios = servicios.Where(s => s.Terapeutas.Any(t => t.TerapeutaId == terapeutaId)).ToList();
                }
                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    servicios = servicios.Where(s =>
                        s.FechaServicio >= fechaInicio.Value &&
                        s.FechaServicio <= fechaFin.Value).ToList();
                }

                return Json(new { success = true, data = servicios });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios del presentador");
                return Json(new { success = false, message = "Error al obtener servicios" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearServicio([FromBody] CreateServicioDto dto)
        {
            try
            {
                // Obtener el ID del usuario actual
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Usuario no identificado" });
                }

                // Obtener el presentador asociado al usuario
                var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/user/{userId}");

                // Asignar el ID del presentador al DTO
                dto.PresentadorId = presentador.Id;

                // Crear el servicio
                var servicio = await _apiService.PostAsync<ServicioDto>("api/Servicio", dto);
                return Json(new { success = true, data = servicio, message = "Servicio creado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear servicio");
                return Json(new { success = false, message = "Error al crear el servicio" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumenSemanal()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/user/{userId}");

                // Calcular fechas de la semana actual
                var fechaInicio = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var fechaFin = fechaInicio.AddDays(6);

                var reporte = await _apiService.GetAsync<ReportePresentadorDetalladoDto>(
                    $"api/Reportes/presentadores/{presentador.Id}?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}");

                return Json(new { success = true, data = reporte });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen semanal");
                return Json(new { success = false, message = "Error al obtener el resumen semanal" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleServicio(int id)
        {
            try
            {
                var servicio = await _apiService.GetAsync<ServicioDto>($"api/Servicio/{id}");
                return PartialView("~/Views/Presentador/Dashboard/_Modal_DetalleServicio.cshtml", servicio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle del servicio");
                return Json(new { success = false, message = "Error al obtener el detalle del servicio" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFormularioServicio(int? id = null)
        {
            try
            {
                if (id.HasValue)
                {
                    var servicio = await _apiService.GetAsync<ServicioDto>($"api/Servicio/{id}");
                    return PartialView("~/Views/Presentador/Dashboard/_Modal_NuevoServicio.cshtml", servicio);
                }
                return PartialView("~/Views/Presentador/Dashboard/_Modal_NuevoServicio.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formulario de servicio");
                return Json(new { success = false, message = "Error al cargar el formulario" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFormularioComprobante(int id)
        {
            try
            {
                var servicio = await _apiService.GetAsync<ServicioDto>($"api/Servicio/{id}");
                var comprobantes = await _apiService.GetAsync<List<ComprobantePagoDto>>($"api/Servicio/{id}/comprobantes");

                var viewModel = new ComprobanteViewModel
                {
                    Servicio = servicio,
                    Comprobantes = comprobantes,
                    TieneComprobantesPagados = comprobantes?.Any(c => c.Estado == "PAGADO") ?? false
                };

                return PartialView("~/Views/Presentador/Dashboard/_Modal_Comprobante.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener formulario de comprobante");
                return Json(new { success = false, message = "Error al cargar el formulario de comprobante" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubirComprobante(int servicioId, [FromForm] CreateComprobantePagoDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioTerapeutaDto>($"api/Servicio/{servicioId}/comprobantes", dto);
                return Json(new { success = true, data = resultado, message = "Comprobante registrado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir comprobante");
                return Json(new { success = false, message = "Error al registrar el comprobante" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumenServicio(int id)
        {
            try
            {
                var servicio = await _apiService.GetAsync<ServicioDto>($"api/Servicio/{id}");
                return PartialView("~/Views/Presentador/Dashboard/_Modal_ResumenServicio.cshtml", servicio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen del servicio");
                return Json(new { success = false, message = "Error al cargar el resumen del servicio" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarServicio(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Servicio/{id}");
                return Json(new { success = true, message = "Servicio eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar servicio");
                return Json(new { success = false, message = "Error al eliminar el servicio" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelarServicio(int id, [FromBody] CancelacionServicioDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioDto>($"api/Servicio/{id}/cancelar", dto);
                return Json(new { success = true, message = "Servicio cancelado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar servicio");
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
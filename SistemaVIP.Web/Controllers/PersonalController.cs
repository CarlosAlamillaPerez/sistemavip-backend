using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Presentador;
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
        public IActionResult Terapeutas()
        {
            return View();
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
                    return PartialView("~/Views/Personal/Presentadores/_Modal_FormPresentador.cshtml", presentador);
                }
                // Para nuevo presentador, pasamos null pero con el modelo correcto
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
                var presentador = await _apiService.GetAsync<PresentadorDto>($"api/Presentador/{id}");
                return PartialView("~/Views/Personal/Presentadores/_Modal_DetallePresentador.cshtml", presentador);
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

        #endregion

        #region Terapeutas
        [HttpGet]
        public async Task<IActionResult> ObtenerTerapeutas()
        {
            try
            {
                var terapeutas = await _apiService.GetAsync<List<dynamic>>("api/Terapeuta/obtener-todos");
                return Json(new { success = true, data = terapeutas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
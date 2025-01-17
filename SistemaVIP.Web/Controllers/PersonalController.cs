using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Presentador;
using SistemaVIP.Core.DTOs.TerapeutaPresentador;
using SistemaVIP.Web.Attributes;
using SistemaVIP.Web.Interfaces;

namespace SistemaVIP.Web.Controllers
{
    [CustomAuthorization("SUPER_ADMIN", "ADMIN")]
    public class PersonalController : Controller
    {
        private readonly IApiService _apiService;

        public PersonalController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Presentadores()
        {
            try
            {
                var presentadores = await _apiService.GetAsync<List<PresentadorDto>>("api/Presentador/obtener-todos");
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
                var presentadores = await _apiService.GetAsync<List<dynamic>>("api/Presentador/obtener-todos");
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
                    return PartialView("_Modal_FormPresentador", presentador);
                }
                return PartialView("_Modal_FormPresentador", null);
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
                return PartialView("_Modal_DetallePresentador", presentador);
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
                var result = await _apiService.PostAsync<PresentadorDto>("api/Presentador/crear", model);
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
        public async Task<IActionResult> ObtenerTerapeutasPorPresentador(int id)
        {
            try
            {
                var terapeutas = await _apiService.GetAsync<List<TerapeutasPorPresentadorDto>>($"api/TerapeutasPresentadores/por-presentador/{id}");
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
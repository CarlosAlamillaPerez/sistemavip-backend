﻿using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Servicio;
using SistemaVIP.Web.Attributes;
using SistemaVIP.Web.Interfaces;
using System.Security.Claims;

namespace SistemaVIP.Web.Controllers
{
    [CustomAuthorization("SUPER_ADMIN", "ADMIN", "PRESENTADOR")]
    public class ServiciosController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<ServiciosController> _logger;

        public ServiciosController(IApiService apiService, ILogger<ServiciosController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Activos()
        {
            return View("~/Views/Servicios/Activos/Index.cshtml");
        }

        [HttpGet]
        public IActionResult Seguimiento()
        {
            return View("~/Views/Servicios/Seguimiento/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> ServiciosExtra()
        {
            return View("~/Views/Servicios/ServicioExtra/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerServiciosActivos()
        {
            try
            {
                var servicios = await _apiService.GetAsync<List<ServicioDto>>("api/Servicio/activos");
                return Json(new { success = true, data = servicios });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerServicio(int id)
        {
            try
            {
                var servicio = await _apiService.GetAsync<ServicioDto>($"api/Servicio/{id}");
                return Json(new { success = true, data = servicio });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerServiciosPorFiltro(string estado = null,int? presentadorId = null,DateTime? fechaInicio = null,DateTime? fechaFin = null)
        {
            try
            {
                var url = "api/Servicio";
                // TODO: Agregar lógica de filtros
                var servicios = await _apiService.GetAsync<List<ServicioDto>>(url);
                return Json(new { success = true, data = servicios });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarInicio(string linkConfirmacion, ConfirmacionServicioDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioTerapeutaDto>(
                    "api/Servicio/confirmar",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarServicio(string linkFinalizacion, FinalizacionServicioDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioTerapeutaDto>(
                    "api/Servicio/finalizar",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerConciliacion(int servicioTerapeutaId)
        {
            try
            {
                var conciliacion = await _apiService.GetAsync<ConciliacionServicioDto>(
                    $"api/Servicio/{servicioTerapeutaId}/conciliacion");
                return Json(new { success = true, data = conciliacion });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RealizarConciliacion(int servicioTerapeutaId)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ResultadoConciliacionDto>(
                    $"api/Servicio/{servicioTerapeutaId}/conciliacion",
                    null);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerComprobantesPago(int servicioTerapeutaId)
        {
            try
            {
                var comprobantes = await _apiService.GetAsync<List<ComprobantePagoDto>>(
                    $"api/Servicio/{servicioTerapeutaId}/comprobantes");
                return Json(new { success = true, data = comprobantes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarComprobante(int servicioTerapeutaId, [FromBody] CreateComprobantePagoDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioTerapeutaDto>(
                    $"api/Servicio/{servicioTerapeutaId}/comprobantes",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarComprobantesMultiples(int servicioTerapeutaId, [FromBody] CreateComprobantesMultiplesDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioTerapeutaDto>(
                    $"api/Servicio/{servicioTerapeutaId}/comprobantes/multiple",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> ActualizarEstadoComprobante(int servicioTerapeutaId,int comprobanteId,[FromBody] UpdateComprobanteEstadoDto dto)
        {
            try
            {
                var resultado = await _apiService.PatchAsync<ServicioTerapeutaDto>(
                    $"api/Servicio/{servicioTerapeutaId}/comprobantes/{comprobanteId}/estado",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarComprobante(int servicioTerapeutaId, int comprobanteId)
        {
            try
            {
                await _apiService.DeleteAsync(
                    $"api/Servicio/{servicioTerapeutaId}/comprobantes/{comprobanteId}");
                return Json(new { success = true, message = "Comprobante eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelarServicio(int id, [FromBody] CancelacionServicioDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioDto>(
                    $"api/Servicio/{id}/cancelar",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        #region ServicioExtra
        [HttpGet]
        public async Task<IActionResult> ObtenerServiciosExtraCatalogo()
        {
            try
            {
                var catalogo = await _apiService.GetAsync<List<ServicioExtraCatalogoDto>>("api/Servicio/servicios-extra/catalogo");
                return Json(new { success = true, data = catalogo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarServicioExtraCatalogo([FromBody] CreateServicioExtraCatalogoDto dto)
        {
            try
            {
                var resultado = await _apiService.PostAsync<ServicioExtraCatalogoDto>(
                    "api/Servicio/servicios-extra/catalogo",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarServicioExtraCatalogo(int id, [FromBody] UpdateServicioExtraCatalogoDto dto)
        {
            try
            {
                var resultado = await _apiService.PutAsync<ServicioExtraCatalogoDto>(
                    $"api/Servicio/servicios-extra/catalogo/{id}",
                    dto);
                return Json(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarServicioExtraCatalogo(int id)
        {
            try
            {
                await _apiService.DeleteAsync($"api/Servicio/servicios-extra/catalogo/{id}");
                return Json(new { success = true, message = "Servicio extra eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerFormularioServicioExtraCatalogo(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    var servicio = await _apiService.GetAsync<ServicioExtraCatalogoDto>(
                        $"api/Servicio/servicios-extra/catalogo/{id.Value}");

                    ViewBag.ServicioExtraId = id.Value;
                    return PartialView("~/Views/Servicios/ServicioExtra/_FormServicioExtra.cshtml", servicio);
                }
                return PartialView("~/Views/Servicios/ServicioExtra/_FormServicioExtra.cshtml", new ServicioExtraCatalogoDto());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleServicioExtraCatalogo(int id)
        {
            try
            {
                var servicio = await _apiService.GetAsync<ServicioExtraCatalogoDto>(
                    $"api/Servicio/servicios-extra/catalogo/{id}");
                return PartialView("~/Views/Servicios/ServicioExtra/_DetalleServicioExtra.cshtml", servicio);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
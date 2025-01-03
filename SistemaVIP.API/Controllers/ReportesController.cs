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

        //SERVICIOS PAGADOS O LIQUIDADOS ↓↓
        [HttpGet("validacion/{presentadorId}")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> ValidarReportePresentador(int presentadorId,[FromQuery] DateTime fechaInicio,[FromQuery] DateTime fechaFin)
        {
            try
            {
                var reporte = await _reportesService.GetReportePresentadorAsync(presentadorId, fechaInicio, fechaFin);

                // Validaciones adicionales
                var validacion = new
                {
                    ReporteOriginal = reporte,
                    Validaciones = new
                    {
                        TotalServiciosCorrectos = reporte.CantidadServicios ==
                            reporte.ServiciosPorDia.Sum(d => d.CantidadServicios),
                        TotalIngresosCorrectos = reporte.TotalIngresosGenerados ==
                            reporte.ServiciosPorDia.Sum(d => d.MontoTotal),
                        TotalComisionesCorrectas = reporte.TotalComisiones ==
                            reporte.ServiciosPorDia.Sum(d => d.Comisiones),
                        TotalPagosCoincide = (reporte.TotalPagosEfectivo + reporte.TotalPagosTransferencia) ==
                            reporte.TotalIngresosGenerados
                    }
                };

                return Ok(validacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("validacion/completa")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> ValidacionCompletaReportes()
        {
            try
            {
                // Período de prueba (octubre a diciembre 2024)
                var fechaInicio = new DateTime(2024, 10, 1);
                var fechaFin = new DateTime(2024, 12, 31);

                // 1. Validación de Presentadores
                var validacionPresentadores = await ValidarReportesPresentadores(fechaInicio, fechaFin);

                // 2. Validación de Terapeutas
                var validacionTerapeutas = await ValidarReportesTerapeutas(fechaInicio, fechaFin);

                // 3. Validación de Servicios Generales
                var validacionServicios = await ValidarReporteServicios(fechaInicio, fechaFin);

                return Ok(new
                {
                    PeriodoValidacion = new { fechaInicio, fechaFin },
                    ResultadosPresentadores = validacionPresentadores,
                    ResultadosTerapeutas = validacionTerapeutas,
                    ResultadosServicios = validacionServicios
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private async Task<object> ValidarReportesPresentadores(DateTime fechaInicio, DateTime fechaFin)
        {
            // Validar Ana Martínez (ID: 1)
            var reporteAna = await _reportesService.GetReportePresentadorAsync(1, fechaInicio, fechaFin);
            var validacionAna = new
            {
                Presentador = "Ana Martínez",
                DatosEsperados = new
                {
                    TotalServicios = 3,
                    TotalIngresos = 7500m, // 2500 + 2200 + 2800
                    TotalComisiones = 1350m, // 450 + 360 + 540
                    ServiciosPorMes = new Dictionary<string, decimal>
            {
                { "2024-10", 2500m },
                { "2024-11", 2200m },
                { "2024-12", 2800m }
            }
                },
                DatosObtenidos = new
                {
                    TotalServicios = reporteAna.CantidadServicios,
                    TotalIngresos = reporteAna.TotalIngresosGenerados,
                    TotalComisiones = reporteAna.TotalComisiones,
                    ServiciosPorMes = reporteAna.ServiciosPorDia
                        .GroupBy(d => new { d.Fecha.Year, d.Fecha.Month })
                        .ToDictionary(
                            g => $"{g.Key.Year}-{g.Key.Month:D2}",
                            g => g.Sum(d => d.MontoTotal)
                        )
                },
                Coincide = new
                {
                    Servicios = reporteAna.CantidadServicios == 3,
                    Ingresos = reporteAna.TotalIngresosGenerados == 7500m,
                    Comisiones = Math.Abs(reporteAna.TotalComisiones - 1350m) < 0.01m
                }
            };

            // Validar Roberto Sánchez (ID: 2)
            var reporteRoberto = await _reportesService.GetReportePresentadorAsync(2, fechaInicio, fechaFin);
            var validacionRoberto = new
            {
                Presentador = "Roberto Sánchez",
                DatosEsperados = new
                {
                    TotalServicios = 3,
                    TotalIngresos = 6100m, // 1800 + 2000 + 2300
                    TotalComisiones = 930m, // 240 + 300 + 390
                    ServiciosPorMes = new Dictionary<string, decimal>
            {
                { "2024-10", 1800m },
                { "2024-11", 2000m },
                { "2024-12", 2300m }
            }
                },
                DatosObtenidos = new
                {
                    TotalServicios = reporteRoberto.CantidadServicios,
                    TotalIngresos = reporteRoberto.TotalIngresosGenerados,
                    TotalComisiones = reporteRoberto.TotalComisiones,
                    ServiciosPorMes = reporteRoberto.ServiciosPorDia
                        .GroupBy(d => new { d.Fecha.Year, d.Fecha.Month })
                        .ToDictionary(
                            g => $"{g.Key.Year}-{g.Key.Month:D2}",
                            g => g.Sum(d => d.MontoTotal)
                        )
                },
                Coincide = new
                {
                    Servicios = reporteRoberto.CantidadServicios == 3,
                    Ingresos = reporteRoberto.TotalIngresosGenerados == 6100m,
                    Comisiones = Math.Abs(reporteRoberto.TotalComisiones - 930m) < 0.01m
                }
            };

            return new { ValidacionAna = validacionAna, ValidacionRoberto = validacionRoberto };
        }

        private async Task<object> ValidarReportesTerapeutas(DateTime fechaInicio, DateTime fechaFin)
        {
            var validaciones = new List<object>();

            // Validar Laura González (ID: 1)
            var reporteLaura = await _reportesService.GetReporteTerapeutaAsync(1, fechaInicio, fechaFin);
            validaciones.Add(new
            {
                Terapeuta = "Laura González",
                DatosEsperados = new
                {
                    TotalServicios = 2,
                    TotalHoras = 3,
                    IngresoBase = 2000m,
                    IngresosExtra = 0m
                },
                DatosObtenidos = new
                {
                    TotalServicios = reporteLaura.TotalServicios,
                    TotalHoras = reporteLaura.TotalHorasTrabajadas,
                    IngresoBase = reporteLaura.IngresosServiciosBase,
                    IngresosExtra = reporteLaura.IngresosServiciosExtra
                }
            });

            // Validar María Hernández (ID: 2)
            var reporteMaria = await _reportesService.GetReporteTerapeutaAsync(2, fechaInicio, fechaFin);
            validaciones.Add(new
            {
                Terapeuta = "María Hernández",
                DatosEsperados = new
                {
                    TotalServicios = 2,
                    TotalHoras = 3,
                    IngresoBase = 2000m,
                    IngresosExtra = 1300m // 500 + 800
                },
                DatosObtenidos = new
                {
                    TotalServicios = reporteMaria.TotalServicios,
                    TotalHoras = reporteMaria.TotalHorasTrabajadas,
                    IngresoBase = reporteMaria.IngresosServiciosBase,
                    IngresosExtra = reporteMaria.IngresosServiciosExtra
                }
            });

            return validaciones;
        }

        private async Task<object> ValidarReporteServicios(DateTime fechaInicio, DateTime fechaFin)
        {
            var reporte = await _reportesService.GetReporteServiciosAsync(fechaInicio, fechaFin);

            return new
            {
                DistribucionEsperada = new
                {
                    Consultorio = new { Total = 3, Monto = 7300m },
                    Domicilio = new { Total = 3, Monto = 6300m }
                },
                DistribucionObtenida = new
                {
                    Consultorio = new
                    {
                        Total = reporte.Distribucion.TotalConsultorio,
                        Monto = reporte.Distribucion.MontoConsultorio
                    },
                    Domicilio = new
                    {
                        Total = reporte.Distribucion.TotalDomicilio,
                        Monto = reporte.Distribucion.MontoDomicilio
                    }
                },
                HorariosPopulares = reporte.HorariosPopulares
                    .OrderByDescending(h => h.CantidadServicios)
                    .Take(3)
                    .Select(h => new
                    {
                        Hora = h.Hora,
                        Cantidad = h.CantidadServicios,
                        MontoPromedio = h.MontoPromedio
                    }),
                ServiciosMultiples = new
                {
                    Individuales = reporte.TiposServicio.ServiciosIndividuales,
                    Multiples = reporte.TiposServicio.ServiciosMultiples,
                    MontoIndividuales = reporte.TiposServicio.MontoServiciosIndividuales,
                    MontoMultiples = reporte.TiposServicio.MontoServiciosMultiples
                }
            };
        }

        //SERVICIOS EN PROCESO O SIN VERIFICAR ↓↓
        [HttpGet("validacion/pendientes")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> ValidarServiciosPendientes()
        {
            try
            {
                // 1. Validar Servicios En Proceso
                var serviciosEnProceso = await ValidarServiciosEnProceso();

                // 2. Validar Comprobantes Pendientes
                var comprobantesPendientes = await ValidarComprobantesPendientes();

                return Ok(new
                {
                    ServiciosEnProceso = serviciosEnProceso,
                    ComprobantesPendientes = comprobantesPendientes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private async Task<object> ValidarServiciosEnProceso()
        {
            var servicios = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .Include(s => s.Presentador)
                .Where(s => s.Estado == EstadosEnum.Servicio.EN_PROCESO)
                .Select(s => new
                {
                    ServicioId = s.Id,
                    FechaInicio = s.FechaServicio,
                    Presentador = $"{s.Presentador.Nombre} {s.Presentador.Apellido}",
                    TipoUbicacion = s.TipoUbicacion,
                    MontoTotal = s.MontoTotal,
                    Terapeuta = s.ServiciosTerapeutas
                        .Select(st => new
                        {
                            Nombre = $"{st.Terapeuta.Nombre} {st.Terapeuta.Apellido}",
                            HoraInicio = st.HoraInicio,
                            DuracionEsperada = s.DuracionHoras,
                            DuracionActual = st.HoraInicio.HasValue ?
                                (DateTime.Now - st.HoraInicio.Value).TotalHours : 0
                        }).FirstOrDefault(),
                    ValidacionesServicio = new
                    {
                        TieneHoraInicio = s.ServiciosTerapeutas
                            .All(st => st.HoraInicio.HasValue),
                        TieneUbicacionInicio = s.ServiciosTerapeutas
                            .All(st => st.UbicacionInicio != null),
                        DuracionDentroLimite = s.ServiciosTerapeutas
                            .All(st => st.HoraInicio.HasValue &&
                                (DateTime.Now - st.HoraInicio.Value).TotalHours <= s.DuracionHoras + 0.5)
                    }
                })
                .ToListAsync();

            return new
            {
                CantidadServiciosEnProceso = servicios.Count,
                Servicios = servicios,
                Alertas = servicios
                    .Where(s => s.Terapeuta.DuracionActual > s.Terapeuta.DuracionEsperada)
                    .Select(s => new
                    {
                        ServicioId = s.ServicioId,
                        Mensaje = $"Servicio excede duración programada: {s.Terapeuta.DuracionActual:F1} hrs vs {s.Terapeuta.DuracionEsperada} hrs programadas"
                    })
            };
        }

        private async Task<object> ValidarComprobantesPendientes()
        {
            var comprobantes = await _context.ComprobantesPago
                .Include(cp => cp.ServicioTerapeuta)
                    .ThenInclude(st => st.Servicio)
                        .ThenInclude(s => s.Presentador)
                .Include(cp => cp.ServicioTerapeuta)
                    .ThenInclude(st => st.Terapeuta)
                .Where(cp => cp.Estado == PagosEnum.EstadoComprobante.POR_CONFIRMAR)
                .Select(cp => new
                {
                    ComprobantePagoId = cp.Id,
                    ServicioId = cp.ServicioTerapeuta.ServicioId,
                    Presentador = $"{cp.ServicioTerapeuta.Servicio.Presentador.Nombre} {cp.ServicioTerapeuta.Servicio.Presentador.Apellido}",
                    Terapeuta = $"{cp.ServicioTerapeuta.Terapeuta.Nombre} {cp.ServicioTerapeuta.Terapeuta.Apellido}",
                    TipoComprobante = cp.TipoComprobante,
                    Monto = cp.Monto,
                    FechaRegistro = cp.FechaRegistro,
                    NumeroOperacion = cp.NumeroOperacion,
                    ValidacionesComprobante = new
                    {
                        TieneComprobante = !string.IsNullOrEmpty(cp.UrlComprobante),
                        MontoValido = cp.Monto > 0,
                        TieneNumeroOperacion = cp.TipoComprobante == PagosEnum.TipoComprobante.TRANSFERENCIA ?
                            !string.IsNullOrEmpty(cp.NumeroOperacion) : true,
                        TiempoEspera = (DateTime.Now - cp.FechaRegistro).TotalHours
                    }
                })
                .ToListAsync();

            // Agrupar por servicio para validar montos totales
            var serviciosConComprobantes = comprobantes
                .GroupBy(c => c.ServicioId)
                .Select(g => new
                {
                    ServicioId = g.Key,
                    Presentador = g.First().Presentador,
                    ComprobantesDetalle = g.Select(c => new
                    {
                        c.ComprobantePagoId,
                        c.TipoComprobante,
                        c.Monto,
                        c.NumeroOperacion
                    }).ToList(),
                    MontoTotalComprobantes = g.Sum(c => c.Monto),
                    ValidacionServicio = new
                    {
                        TotalComprobantes = g.Count(),
                        RequiereRevisionUrgente = g.Any(c => c.ValidacionesComprobante.TiempoEspera > 24)
                    }
                })
                .ToList();

            return new
            {
                TotalComprobantesPendientes = comprobantes.Count,
                ComprobantesPorServicio = serviciosConComprobantes,
                Alertas = serviciosConComprobantes
                    .Where(s => s.ValidacionServicio.RequiereRevisionUrgente)
                    .Select(s => new
                    {
                        ServicioId = s.ServicioId,
                        Mensaje = $"Comprobantes pendientes por más de 24 horas para el servicio {s.ServicioId}"
                    })
            };
        }

        //SERVICIOS EN PROCESO O SIN VERIFICAR (RESUMEN) ↓↓

        [HttpGet("validacion/resumen-pendientes")]
        [Authorize(Roles = $"{UserRoles.SUPER_ADMIN}, {UserRoles.ADMIN}")]
        public async Task<ActionResult> GetResumenValidacionesPendientes()
        {
            try
            {
                // 1. Servicios en proceso que requieren atención
                var serviciosEnProceso = await _context.Servicios
                    .Include(s => s.ServiciosTerapeutas)
                        .ThenInclude(st => st.Terapeuta)
                    .Include(s => s.Presentador)
                    .Where(s => s.Estado == EstadosEnum.Servicio.EN_PROCESO)
                    .Select(s => new
                    {
                        ServicioId = s.Id,
                        FechaInicio = s.ServiciosTerapeutas
                            .FirstOrDefault().HoraInicio,
                        Presentador = $"{s.Presentador.Nombre} {s.Presentador.Apellido}",
                        Terapeuta = s.ServiciosTerapeutas
                            .Select(st => st.Terapeuta.Nombre)
                            .FirstOrDefault(),
                        DuracionProgramada = s.DuracionHoras,
                        DuracionActual = s.ServiciosTerapeutas
                            .Where(st => st.HoraInicio.HasValue)
                            .Select(st => (DateTime.Now - st.HoraInicio.Value).TotalHours)
                            .FirstOrDefault(),
                        RequiereAtencion = s.ServiciosTerapeutas
                            .Any(st => st.HoraInicio.HasValue &&
                                 (DateTime.Now - st.HoraInicio.Value).TotalHours > s.DuracionHoras + 0.5)
                    })
                    .ToListAsync();

                // 2. Comprobantes pendientes de verificación
                var comprobantesPendientes = await _context.ComprobantesPago
                    .Include(cp => cp.ServicioTerapeuta)
                        .ThenInclude(st => st.Servicio)
                            .ThenInclude(s => s.Presentador)
                    .Where(cp => cp.Estado == PagosEnum.EstadoComprobante.POR_CONFIRMAR)
                    .Select(cp => new
                    {
                        ComprobantePagoId = cp.Id,
                        ServicioId = cp.ServicioTerapeuta.ServicioId,
                        Presentador = cp.ServicioTerapeuta.Servicio.Presentador.Nombre,
                        TipoComprobante = cp.TipoComprobante,
                        Monto = cp.Monto,
                        HorasEspera = (DateTime.Now - cp.FechaRegistro).TotalHours,
                        RequiereAtencionUrgente = (DateTime.Now - cp.FechaRegistro).TotalHours > 24
                    })
                    .ToListAsync();

                // 3. Servicios finalizados sin comprobantes
                var serviciosFinalizadosSinComprobantes = await _context.ServiciosTerapeutas
                    .Include(st => st.Servicio)
                        .ThenInclude(s => s.Presentador)
                    .Include(st => st.ComprobantesPago)
                    .Where(st => st.Estado == EstadosEnum.Servicio.FINALIZADO &&
                           !st.ComprobantesPago.Any())
                    .Select(st => new
                    {
                        ServicioId = st.ServicioId,
                        Presentador = st.Servicio.Presentador.Nombre,
                        FechaFinalizacion = st.HoraFin,
                        HorasSinComprobante = st.HoraFin.HasValue ?
                            (DateTime.Now - st.HoraFin.Value).TotalHours : 0
                    })
                    .ToListAsync();

                return Ok(new
                {
                    FechaReporte = DateTime.Now,
                    ResumenGeneral = new
                    {
                        TotalServiciosEnProceso = serviciosEnProceso.Count,
                        ServiciosRequierenAtencion = serviciosEnProceso
                            .Count(s => s.RequiereAtencion),
                        TotalComprobantesPendientes = comprobantesPendientes.Count,
                        ComprobantesPendientesUrgentes = comprobantesPendientes
                            .Count(c => c.RequiereAtencionUrgente),
                        ServiciosFinalizadosSinComprobantes = serviciosFinalizadosSinComprobantes.Count
                    },
                    AlertasUrgentes = new
                    {
                        ServiciosExtendidos = serviciosEnProceso
                            .Where(s => s.RequiereAtencion)
                            .Select(s => new
                            {
                                s.ServicioId,
                                s.Presentador,
                                s.Terapeuta,
                                DuracionExcedida = s.DuracionActual - s.DuracionProgramada,
                                Mensaje = $"Servicio excede tiempo programado por {(s.DuracionActual - s.DuracionProgramada):F1} horas"
                            }),
                        ComprobantesPrioritarios = comprobantesPendientes
                            .Where(c => c.RequiereAtencionUrgente)
                            .OrderByDescending(c => c.HorasEspera)
                            .Select(c => new
                            {
                                c.ServicioId,
                                c.Presentador,
                                c.TipoComprobante,
                                c.Monto,
                                HorasEspera = c.HorasEspera,
                                Mensaje = $"Comprobante pendiente por {c.HorasEspera:F1} horas"
                            }),
                        ServiciosSinComprobantes = serviciosFinalizadosSinComprobantes
                            .Where(s => s.HorasSinComprobante > 12)
                            .OrderByDescending(s => s.HorasSinComprobante)
                            .Select(s => new
                            {
                                s.ServicioId,
                                s.Presentador,
                                HorasDesdeFinalizacion = s.HorasSinComprobante,
                                Mensaje = $"Servicio finalizado sin comprobantes por {s.HorasSinComprobante:F1} horas"
                            })
                    },
                    EstadisticasPorPresentador = await GetEstadisticasPorPresentador
                    (
                        serviciosEnProceso.Select(s => new ServicioEnProcesoDto
                        {
                            Presentador = s.Presentador,
                            RequiereAtencion = s.RequiereAtencion
                        }),
                        comprobantesPendientes.Select(c => new ComprobantePendienteDto
                        {
                            Presentador = c.Presentador,
                            RequiereAtencionUrgente = c.RequiereAtencionUrgente
                        }),
                        serviciosFinalizadosSinComprobantes.Select(s => new ServicioSinComprobanteDto
                        {
                            Presentador = s.Presentador
                        })
                    )
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class ServicioEnProcesoDto
        {
            public string Presentador { get; set; }
            public bool RequiereAtencion { get; set; }
        }

        public class ComprobantePendienteDto
        {
            public string Presentador { get; set; }
            public bool RequiereAtencionUrgente { get; set; }
        }

        public class ServicioSinComprobanteDto
        {
            public string Presentador { get; set; }
        }

        private async Task<object> GetEstadisticasPorPresentador(IEnumerable<ServicioEnProcesoDto> serviciosEnProceso, IEnumerable<ComprobantePendienteDto> comprobantesPendientes,IEnumerable<ServicioSinComprobanteDto> serviciosSinComprobantes)
        {
            var presentadores = await _context.Presentadores
                .Where(p => p.Estado == EstadosEnum.General.ACTIVO)
                .Select(p => new
                {
                    p.Id,
                    NombreCompleto = $"{p.Nombre} {p.Apellido}",
                    Estadisticas = new
                    {
                        ServiciosEnProceso = serviciosEnProceso
                            .Cast<ServicioEnProcesoDto>()
                            .Count(s => s.Presentador == $"{p.Nombre} {p.Apellido}"),
                        ServiciosRequierenAtencion = serviciosEnProceso
                            .Cast<ServicioEnProcesoDto>()
                            .Count(s => s.Presentador == $"{p.Nombre} {p.Apellido}" && s.RequiereAtencion),
                        ComprobantesPendientes = comprobantesPendientes
                            .Cast<ComprobantePendienteDto>()
                            .Count(c => c.Presentador == p.Nombre),
                        ComprobantesPendientesUrgentes = comprobantesPendientes
                            .Cast<ComprobantePendienteDto>()
                            .Count(c => c.Presentador == p.Nombre && c.RequiereAtencionUrgente),
                        ServiciosSinComprobantes = serviciosSinComprobantes
                            .Cast<ServicioSinComprobanteDto>()
                            .Count(s => s.Presentador == p.Nombre)
                    }
                })
                .ToListAsync();

            return presentadores;
        }



    }
}
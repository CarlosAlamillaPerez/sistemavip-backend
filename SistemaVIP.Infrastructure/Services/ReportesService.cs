// ReportesService.cs
using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs.Reportes;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;

namespace SistemaVIP.Infrastructure.Services
{
    public class ReportesService : IReportesService
    {
        private readonly ApplicationDbContext _context;

        public ReportesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReportePresentadorDto>> GetReportePresentadoresAsync(ReportesPresentadorFiltroDto filtro)
        {
            var query = _context.Presentadores
                .Include(p => p.User)
                .AsQueryable();

            // Aplicar filtro por presentador específico si se proporciona
            if (filtro.PresentadorId.HasValue)
            {
                query = query.Where(p => p.Id == filtro.PresentadorId.Value);
            }

            var presentadores = await query.ToListAsync();
            var reportes = new List<ReportePresentadorDto>();

            foreach (var presentador in presentadores)
            {
                // Obtener solo servicios LIQUIDADOS o PAGADOS
                var servicios = await _context.Servicios
                    .Include(s => s.ServiciosTerapeutas)
                        .ThenInclude(st => st.ServiciosExtra)
                    .Include(s => s.ServiciosTerapeutas)
                        .ThenInclude(st => st.ComprobantesPago)
                    .Where(s => s.PresentadorId == presentador.Id &&
                               s.FechaServicio >= filtro.FechaInicio &&
                               s.FechaServicio <= filtro.FechaFin &&
                               (s.Estado == EstadosEnum.Servicio.LIQUIDADO ||
                                s.Estado == EstadosEnum.Servicio.PAGADO))
                    .OrderByDescending(s => s.FechaServicio)
                    .ToListAsync();

                var serviciosPorDia = servicios
                    .GroupBy(s => s.FechaServicio.Date)
                    .Select(g => new ServiciosPorDiaDto
                    {
                        Fecha = g.Key,
                        CantidadServicios = g.Count(),
                        MontoTotal = g.Sum(s => s.MontoTotal),
                        Comisiones = g.Sum(s =>
                            _context.Comisiones
                                .Where(c => c.ServicioId == s.Id &&
                                          c.Estado == EstadosEnum.Comision.LIQUIDADO)
                                .Select(c => c.MontoComisionPresentador)
                                .FirstOrDefault())
                    })
                    .OrderByDescending(x => x.Fecha)
                    .ToList();

                reportes.Add(new ReportePresentadorDto
                {
                    PresentadorId = presentador.Id,
                    NombrePresentador = $"{presentador.Nombre} {presentador.Apellido}".Trim(),
                    TotalIngresosGenerados = servicios.Sum(s => s.MontoTotal),
                    TotalComisiones = servicios
                        .Sum(s => _context.Comisiones
                            .Where(c => c.ServicioId == s.Id &&
                                       c.Estado == EstadosEnum.Comision.LIQUIDADO)
                            .Select(c => c.MontoComisionPresentador)
                            .FirstOrDefault()),
                    CantidadServicios = servicios.Count,
                    TotalPagosEfectivo = servicios
                        .SelectMany(s => s.ServiciosTerapeutas)
                        .SelectMany(st => st.ComprobantesPago)
                        .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.EFECTIVO &&
                                    cp.Estado == PagosEnum.EstadoComprobante.PAGADO)
                        .Sum(cp => cp.Monto),
                    TotalPagosTransferencia = servicios
                        .SelectMany(s => s.ServiciosTerapeutas)
                        .SelectMany(st => st.ComprobantesPago)
                        .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.TRANSFERENCIA &&
                                    cp.Estado == PagosEnum.EstadoComprobante.PAGADO)
                        .Sum(cp => cp.Monto),
                    ServiciosPorDia = serviciosPorDia
                });
            }

            return reportes;
        }

        public async Task<ReportePresentadorDetalladoDto> GetReportePresentadorAsync(int presentadorId,DateTime fechaInicio,DateTime fechaFin)
        {
            var presentador = await _context.Presentadores
                .FirstOrDefaultAsync(p => p.Id == presentadorId);

            if (presentador == null)
                throw new InvalidOperationException("Presentador no encontrado");

            // Obtener TODOS los servicios en el rango de fechas
            var servicios = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ServiciosExtra)
                        .ThenInclude(se => se.ServicioExtraCatalogo)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ComprobantesPago)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .Where(s => s.PresentadorId == presentadorId &&
                           s.FechaServicio >= fechaInicio &&
                           s.FechaServicio <= fechaFin)
                .OrderByDescending(s => s.FechaServicio)
                .ToListAsync();

            // Agrupar por estado
            var serviciosPorEstado = servicios
                .GroupBy(s => s.Estado)
                .Select(g => new ServicioPorEstadoDto
                {
                    Estado = g.Key,
                    CantidadServicios = g.Count(),
                    MontoTotal = g.Sum(s => s.MontoTotal),
                    Servicios = g.Select(s => MapToServicioDetallado(s)).ToList()
                })
                .ToList();

            // Agrupar por día con más detalles
            var serviciosPorDia = servicios
                .GroupBy(s => s.FechaServicio.Date)
                .Select(g => new ServicioPorDiaDetalladoDto
                {
                    Fecha = g.Key,
                    CantidadServicios = g.Count(),
                    MontoTotal = g.Sum(s => s.MontoTotal),
                    Comisiones = g.Sum(s =>
                        _context.Comisiones
                            .Where(c => c.ServicioId == s.Id)
                            .Select(c => c.MontoComisionPresentador)
                            .FirstOrDefault()),
                    Estados = g.Select(s => s.Estado).Distinct().ToList(),
                    ServiciosEnProceso = g.Count(s => s.Estado == EstadosEnum.Servicio.EN_PROCESO),
                    ServiciosPendientes = g.Count(s => s.Estado == EstadosEnum.Servicio.POR_CONFIRMAR),
                    ServiciosFinalizados = g.Count(s => s.Estado == EstadosEnum.Servicio.FINALIZADO),
                    MontoServiciosExtra = g.Sum(s =>
                        s.ServiciosTerapeutas.Sum(st =>
                            st.ServiciosExtra.Sum(se => se.Monto)))
                })
                .OrderByDescending(x => x.Fecha)
                .ToList();

            // Resumen del día actual
            var hoy = DateTime.Now.Date;
            var serviciosHoy = servicios.Where(s => s.FechaServicio.Date == hoy).ToList();

            var resumenHoy = new ResumenDiarioDto
            {
                ServiciosActivos = serviciosHoy.Count(s => s.Estado == EstadosEnum.Servicio.EN_PROCESO),
                ServiciosPendientesVerificacion = serviciosHoy.Count(s =>
                    s.Estado == EstadosEnum.Servicio.POR_CONFIRMAR ||
                    s.ServiciosTerapeutas.Any(st =>
                        st.ComprobantesPago.Any(cp =>
                            cp.Estado == PagosEnum.EstadoComprobante.POR_CONFIRMAR))),
                MontoTotalDia = serviciosHoy.Sum(s => s.MontoTotal),
                MontoServiciosExtra = serviciosHoy.Sum(s =>
                    s.ServiciosTerapeutas.Sum(st =>
                        st.ServiciosExtra.Sum(se => se.Monto))),
                ServiciosEnProceso = serviciosHoy
                    .Where(s => s.Estado == EstadosEnum.Servicio.EN_PROCESO)
                    .Select(s => MapToServicioDetallado(s))
                    .ToList()
            };

            return new ReportePresentadorDetalladoDto
            {
                PresentadorId = presentador.Id,
                NombrePresentador = $"{presentador.Nombre} {presentador.Apellido}".Trim(),
                TotalIngresosGenerados = servicios.Sum(s => s.MontoTotal),
                TotalComisiones = servicios.Sum(s =>
                    _context.Comisiones
                        .Where(c => c.ServicioId == s.Id)
                        .Select(c => c.MontoComisionPresentador)
                        .FirstOrDefault()),
                CantidadServicios = servicios.Count,
                TotalPagosEfectivo = servicios
                    .SelectMany(s => s.ServiciosTerapeutas)
                    .SelectMany(st => st.ComprobantesPago)
                    .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.EFECTIVO &&
                                cp.Estado == PagosEnum.EstadoComprobante.PAGADO)
                    .Sum(cp => cp.Monto),
                TotalPagosTransferencia = servicios
                    .SelectMany(s => s.ServiciosTerapeutas)
                    .SelectMany(st => st.ComprobantesPago)
                    .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.TRANSFERENCIA &&
                                cp.Estado == PagosEnum.EstadoComprobante.PAGADO)
                    .Sum(cp => cp.Monto),
                ServiciosPorEstado = serviciosPorEstado,
                ServiciosPorDia = serviciosPorDia,
                ResumenHoy = resumenHoy
            };
        }

        private ServicioDetalladoDto MapToServicioDetallado(ServiciosModel servicio)
        {
            return new ServicioDetalladoDto
            {
                ServicioId = servicio.Id,
                FechaServicio = servicio.FechaServicio,
                TipoUbicacion = servicio.TipoUbicacion,
                MontoTotal = servicio.MontoTotal,
                Estado = servicio.Estado,
                ServiciosExtra = servicio.ServiciosTerapeutas
                    .SelectMany(st => st.ServiciosExtra)
                    .Select(se => new ServicioExtraResumenDto
                    {
                        Nombre = se.ServicioExtraCatalogo.Nombre,
                        Monto = se.Monto
                    }).ToList(),
                Comprobantes = servicio.ServiciosTerapeutas
                    .SelectMany(st => st.ComprobantesPago)
                    .Select(cp => new ComprobantePagoResumenDto
                    {
                        TipoComprobante = cp.TipoComprobante,
                        Monto = cp.Monto,
                        Estado = cp.Estado
                    }).ToList()
            };
        }

        public async Task<List<ReporteTerapeutaResumenDto>> GetReporteTerapeutasAsync(ReporteTerapeutaFiltroDto filtro)
        {
            var query = _context.Terapeutas.AsQueryable();

            if (filtro.TerapeutaId.HasValue)
            {
                query = query.Where(t => t.Id == filtro.TerapeutaId.Value);
            }

            var terapeutas = await query.ToListAsync();
            var reportes = new List<ReporteTerapeutaResumenDto>();

            foreach (var terapeuta in terapeutas)
            {
                // Obtener solo servicios LIQUIDADOS o PAGADOS
                var serviciosTerapeuta = await _context.ServiciosTerapeutas
                    .Include(st => st.Servicio)
                    .Include(st => st.ServiciosExtra)
                    .Where(st => st.TerapeutaId == terapeuta.Id &&
                                st.Servicio.FechaServicio >= filtro.FechaInicio &&
                                st.Servicio.FechaServicio <= filtro.FechaFin &&
                                (st.Servicio.Estado == EstadosEnum.Servicio.LIQUIDADO ||
                                 st.Servicio.Estado == EstadosEnum.Servicio.PAGADO))
                    .ToListAsync();

                // Calcular proporción de servicios
                var proporcion = new ProporcionServiciosDto
                {
                    ServiciosConsultorio = serviciosTerapeuta.Count(st =>
                        st.Servicio.TipoUbicacion == ServicioEnum.TipoUbicacion.CONSULTORIO),
                    ServiciosDomicilio = serviciosTerapeuta.Count(st =>
                        st.Servicio.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO),
                    IngresosConsultorio = serviciosTerapeuta
                        .Where(st => st.Servicio.TipoUbicacion == ServicioEnum.TipoUbicacion.CONSULTORIO)
                        .Sum(st => st.MontoTerapeuta ?? 0),
                    IngresosDomicilio = serviciosTerapeuta
                        .Where(st => st.Servicio.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO)
                        .Sum(st => st.MontoTerapeuta ?? 0)
                };

                // Agrupar por día
                var serviciosPorDia = serviciosTerapeuta
                    .GroupBy(st => st.Servicio.FechaServicio.Date)
                    .Select(g => new ServicioTerapeutaDiaDto
                    {
                        Fecha = g.Key,
                        CantidadServicios = g.Count(),
                        HorasTrabajadas = g.Where(st => st.HoraInicio.HasValue && st.HoraFin.HasValue)
                            .Sum(st => (int)(st.HoraFin.Value - st.HoraInicio.Value).TotalHours),
                        MontoServiciosBase = g.Sum(st => st.MontoTerapeuta ?? 0),
                        MontoServiciosExtra = g.Sum(st => st.ServiciosExtra.Sum(se => se.Monto))
                    })
                    .OrderByDescending(x => x.Fecha)
                    .ToList();

                var reporte = new ReporteTerapeutaResumenDto
                {
                    TerapeutaId = terapeuta.Id,
                    NombreTerapeuta = $"{terapeuta.Nombre} {terapeuta.Apellido}".Trim(),
                    TotalServicios = serviciosTerapeuta.Count,
                    IngresosServiciosBase = serviciosTerapeuta.Sum(st => st.MontoTerapeuta ?? 0),
                    IngresosServiciosExtra = serviciosTerapeuta.Sum(st =>
                        st.ServiciosExtra.Sum(se => se.Monto)),
                    TotalIngresos = serviciosTerapeuta.Sum(st => (st.MontoTerapeuta ?? 0) +
                        st.ServiciosExtra.Sum(se => se.Monto)),
                    HorasTrabajadas = serviciosTerapeuta
                        .Where(st => st.HoraInicio.HasValue && st.HoraFin.HasValue)
                        .Sum(st => (int)(st.HoraFin.Value - st.HoraInicio.Value).TotalHours),
                    ProporcionServicios = proporcion,
                    ServiciosPorDia = serviciosPorDia
                };

                reportes.Add(reporte);
            }

            return reportes;
        }

        public async Task<ReporteTerapeutaDetalladoDto> GetReporteTerapeutaAsync(int terapeutaId,DateTime fechaInicio,DateTime fechaFin)
        {
            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.Id == terapeutaId);

            if (terapeuta == null)
                throw new InvalidOperationException("Terapeuta no encontrada");

            // Obtener TODOS los servicios en el rango de fechas
            var serviciosTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                    .ThenInclude(s => s.Presentador)
                .Include(st => st.ServiciosExtra)
                    .ThenInclude(se => se.ServicioExtraCatalogo)
                .Include(st => st.ComprobantesPago)
                .Where(st => st.TerapeutaId == terapeutaId &&
                            st.Servicio.FechaServicio >= fechaInicio &&
                            st.Servicio.FechaServicio <= fechaFin)
                .OrderByDescending(st => st.Servicio.FechaServicio)
                .ToListAsync();

            // Agrupar por estado
            var serviciosPorEstado = serviciosTerapeuta
                .GroupBy(st => st.Servicio.Estado)
                .Select(g => new ServicioPorEstadoTerapeutaDto
                {
                    Estado = g.Key,
                    CantidadServicios = g.Count(),
                    MontoBase = g.Sum(st => st.MontoTerapeuta ?? 0),
                    MontoExtra = g.Sum(st => st.ServiciosExtra.Sum(se => se.Monto)),
                    Servicios = g.Select(st => MapToServicioDetalladoTerapeuta(st)).ToList()
                })
                .ToList();

            // Agrupar por día
            var serviciosPorDia = serviciosTerapeuta
                .GroupBy(st => st.Servicio.FechaServicio.Date)
                .Select(g => new ServicioPorDiaTerapeutaDto
                {
                    Fecha = g.Key,
                    CantidadServicios = g.Count(),
                    MontoBase = g.Sum(st => st.MontoTerapeuta ?? 0),
                    MontoExtra = g.Sum(st => st.ServiciosExtra.Sum(se => se.Monto)),
                    ServiciosEnProceso = g.Count(st => st.Servicio.Estado == EstadosEnum.Servicio.EN_PROCESO),
                    ServiciosPendientes = g.Count(st => st.Servicio.Estado == EstadosEnum.Servicio.POR_CONFIRMAR),
                    ServiciosFinalizados = g.Count(st => st.Servicio.Estado == EstadosEnum.Servicio.FINALIZADO)
                })
                .OrderByDescending(x => x.Fecha)
                .ToList();

            // Agrupar por presentador
            var serviciosPorPresentador = serviciosTerapeuta
                .GroupBy(st => st.Servicio.Presentador)
                .Select(g => new ServicioPorPresentadorDto
                {
                    PresentadorId = g.Key.Id,
                    NombrePresentador = $"{g.Key.Nombre} {g.Key.Apellido}".Trim(),
                    CantidadServicios = g.Count(),
                    MontoTotal = g.Sum(st => (st.MontoTerapeuta ?? 0) + st.ServiciosExtra.Sum(se => se.Monto)),
                    Servicios = g.Select(st => MapToServicioDetalladoTerapeuta(st)).ToList()
                })
                .ToList();

            // Resumen del día actual
            var hoy = DateTime.Now.Date;
            var serviciosHoy = serviciosTerapeuta
                .Where(st => st.Servicio.FechaServicio.Date == hoy)
                .ToList();

            var resumenHoy = new ResumenDiarioTerapeutaDto
            {
                ServiciosActivos = serviciosHoy.Count(st =>
                    st.Servicio.Estado == EstadosEnum.Servicio.EN_PROCESO),
                ServiciosPendientesVerificacion = serviciosHoy.Count(st =>
                    st.Servicio.Estado == EstadosEnum.Servicio.POR_CONFIRMAR ||
                    st.ComprobantesPago.Any(cp =>
                        cp.Estado == PagosEnum.EstadoComprobante.POR_CONFIRMAR)),
                MontoBaseHoy = serviciosHoy.Sum(st => st.MontoTerapeuta ?? 0),
                MontoExtrasHoy = serviciosHoy.Sum(st =>
                    st.ServiciosExtra.Sum(se => se.Monto)),
                ServiciosEnProceso = serviciosHoy
                    .Where(st => st.Servicio.Estado == EstadosEnum.Servicio.EN_PROCESO)
                    .Select(st => MapToServicioDetalladoTerapeuta(st))
                    .ToList()
            };

            return new ReporteTerapeutaDetalladoDto
            {
                TerapeutaId = terapeuta.Id,
                NombreTerapeuta = $"{terapeuta.Nombre} {terapeuta.Apellido}".Trim(),
                TotalIngresos = serviciosTerapeuta.Sum(st =>
                    (st.MontoTerapeuta ?? 0) + st.ServiciosExtra.Sum(se => se.Monto)),
                IngresosBase = serviciosTerapeuta.Sum(st => st.MontoTerapeuta ?? 0),
                IngresosExtra = serviciosTerapeuta.Sum(st =>
                    st.ServiciosExtra.Sum(se => se.Monto)),
                ServiciosPorEstado = serviciosPorEstado,
                ServiciosPorDia = serviciosPorDia,
                ServiciosPorPresentador = serviciosPorPresentador,
                ResumenHoy = resumenHoy
            };
        }

        private ServicioDetalladoTerapeutaDto MapToServicioDetalladoTerapeuta(ServiciosTerapeutasModel servicioTerapeuta)
        {
            return new ServicioDetalladoTerapeutaDto
            {
                ServicioId = servicioTerapeuta.ServicioId,
                FechaServicio = servicioTerapeuta.Servicio.FechaServicio,
                TipoUbicacion = servicioTerapeuta.Servicio.TipoUbicacion,
                NombrePresentador = $"{servicioTerapeuta.Servicio.Presentador.Nombre} {servicioTerapeuta.Servicio.Presentador.Apellido}".Trim(),
                MontoBase = servicioTerapeuta.MontoTerapeuta ?? 0,
                Estado = servicioTerapeuta.Servicio.Estado,
                ServiciosExtra = servicioTerapeuta.ServiciosExtra
                    .Select(se => new ServicioExtraResumenDto
                    {
                        Nombre = se.ServicioExtraCatalogo.Nombre,
                        Monto = se.Monto,
                        FechaRegistro = se.FechaRegistro,
                        Notas = se.Notas
                    }).ToList(),
                Comprobantes = servicioTerapeuta.ComprobantesPago
                    .Select(cp => new ComprobantePagoResumenDto
                    {
                        TipoComprobante = cp.TipoComprobante,
                        Monto = cp.Monto,
                        Estado = cp.Estado,
                        FechaRegistro = cp.FechaRegistro,
                        NumeroOperacion = cp.NumeroOperacion
                    }).ToList(),
                HoraInicio = servicioTerapeuta.HoraInicio,
                HoraFin = servicioTerapeuta.HoraFin
            };
        }

        private async Task<ReporteTerapeutaDto> GenerarReporteTerapeuta(int terapeutaId,string nombre,string apellido,DateTime fechaInicio,DateTime fechaFin)
        {
            var serviciosTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.ServiciosExtra)
                .Where(st => st.TerapeutaId == terapeutaId &&
                            st.Servicio.FechaServicio >= fechaInicio &&
                            st.Servicio.FechaServicio <= fechaFin)
                .ToListAsync();

            // Calcular totales
            decimal ingresosBase = serviciosTerapeuta.Sum(st => st.MontoTerapeuta ?? 0);
            decimal ingresosExtra = serviciosTerapeuta
                .SelectMany(st => st.ServiciosExtra)
                .Sum(se => se.Monto);

            // Calcular horas totales trabajadas
            int horasTotales = serviciosTerapeuta
                .Where(st => st.HoraInicio.HasValue && st.HoraFin.HasValue)
                .Sum(st => (int)(st.HoraFin.Value - st.HoraInicio.Value).TotalHours);

            // Agrupar por día
            var serviciosPorDia = serviciosTerapeuta
                .GroupBy(st => st.Servicio.FechaServicio.Date)
                .Select(g => new ServicioTerapeutaDiaDto
                {
                    Fecha = g.Key,
                    CantidadServicios = g.Count(),
                    HorasTrabajadas = g.Where(st => st.HoraInicio.HasValue && st.HoraFin.HasValue)
                                      .Sum(st => (int)(st.HoraFin.Value - st.HoraInicio.Value).TotalHours),
                    MontoServiciosBase = g.Sum(st => st.MontoTerapeuta ?? 0),
                    MontoServiciosExtra = g.SelectMany(st => st.ServiciosExtra).Sum(se => se.Monto)
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            return new ReporteTerapeutaDto
            {
                TerapeutaId = terapeutaId,
                NombreTerapeuta = $"{nombre} {apellido}".Trim(),
                TotalServicios = serviciosTerapeuta.Count,
                TotalHorasTrabajadas = horasTotales,
                IngresosServiciosBase = ingresosBase,
                IngresosServiciosExtra = ingresosExtra,
                TotalIngresos = ingresosBase + ingresosExtra,
                ServiciosPorDia = serviciosPorDia
            };
        }

        public async Task<ReporteServiciosDto> GetReporteServiciosAsync(DateTime fechaInicio,DateTime fechaFin)
        {
            var servicios = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ServiciosExtra)
                .Where(s => s.FechaServicio >= fechaInicio &&
                           s.FechaServicio <= fechaFin &&
                           (s.Estado == EstadosEnum.Servicio.LIQUIDADO ||
                            s.Estado == EstadosEnum.Servicio.PAGADO))
                .ToListAsync();

            // Distribución por tipo de ubicación
            var distribucion = new DistribucionServiciosDto
            {
                TotalConsultorio = servicios.Count(s =>
                    s.TipoUbicacion == ServicioEnum.TipoUbicacion.CONSULTORIO),
                TotalDomicilio = servicios.Count(s =>
                    s.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO),
                MontoConsultorio = servicios
                    .Where(s => s.TipoUbicacion == ServicioEnum.TipoUbicacion.CONSULTORIO)
                    .Sum(s => s.MontoTotal),
                MontoDomicilio = servicios
                    .Where(s => s.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO)
                    .Sum(s => s.MontoTotal)
            };

            // Análisis de horarios
            var horarios = servicios
                .GroupBy(s => s.FechaServicio.Hour)
                .Select(g => new HorarioServiciosDto
                {
                    Hora = g.Key,
                    CantidadServicios = g.Count(),
                    MontoPromedio = g.Average(s => s.MontoTotal)
                })
                .OrderBy(h => h.Hora)
                .ToList();

            // Análisis de tipos (individual vs múltiple)
            var serviciosIndividuales = servicios
                .Where(s => s.ServiciosTerapeutas.Count == 1).ToList();
            var serviciosMultiples = servicios
                .Where(s => s.ServiciosTerapeutas.Count > 1).ToList();

            var tiposServicio = new TiposServicioDto
            {
                ServiciosIndividuales = serviciosIndividuales.Count,
                ServiciosMultiples = serviciosMultiples.Count,
                MontoServiciosIndividuales = serviciosIndividuales.Sum(s => s.MontoTotal),
                MontoServiciosMultiples = serviciosMultiples.Sum(s => s.MontoTotal)
            };

            return new ReporteServiciosDto
            {
                Distribucion = distribucion,
                HorariosPopulares = horarios,
                TiposServicio = tiposServicio
            };
        }



    }
}
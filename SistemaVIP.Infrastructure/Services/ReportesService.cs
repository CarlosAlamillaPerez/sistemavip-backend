// ReportesService.cs
using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs.Reportes;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
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
                var reporte = await GenerarReportePresentador(
                    presentador.Id,
                    presentador.Nombre,
                    presentador.Apellido,
                    filtro.FechaInicio,
                    filtro.FechaFin);

                reportes.Add(reporte);
            }

            return reportes;
        }

        public async Task<ReportePresentadorDto> GetReportePresentadorAsync(int presentadorId,DateTime fechaInicio,DateTime fechaFin)
        {
            // Primero validar que el presentador exista
            var presentador = await _context.Presentadores
                .FirstOrDefaultAsync(p => p.Id == presentadorId);

            if (presentador == null) throw new InvalidOperationException("Presentador no encontrado");

            // Obtener servicios pero solo LIQUIDADOS o PAGADOS
            var servicios = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ServiciosExtra)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ComprobantesPago)
                .Where(s => s.PresentadorId == presentadorId &&
                           s.FechaServicio >= fechaInicio &&
                           s.FechaServicio <= fechaFin &&
                           (s.Estado == EstadosEnum.Servicio.LIQUIDADO ||
                            s.Estado == EstadosEnum.Servicio.PAGADO))
                .OrderByDescending(s => s.FechaServicio)
                .ToListAsync();

            // Calcular totales
            decimal totalIngresosGenerados = 0;
            decimal totalComisiones = 0;
            var serviciosPorDia = new List<ServiciosPorDiaDto>();
            var serviciosAgrupados = servicios.GroupBy(s => s.FechaServicio.Date);

            foreach (var grupo in serviciosAgrupados)
            {
                var serviciosDia = grupo.ToList();
                var montoDia = 0m;
                var comisionesDia = 0m;

                foreach (var servicio in serviciosDia)
                {
                    // Sumar monto base del servicio
                    montoDia += servicio.MontoTotal;

                    // Sumar servicios extra si existen
                    var serviciosExtra = servicio.ServiciosTerapeutas
                        .SelectMany(st => st.ServiciosExtra)
                        .Sum(se => se.Monto);
                    montoDia += serviciosExtra;

                    // Calcular comisión (30% del margen)
                    var montoTerapeuta = servicio.ServiciosTerapeutas
                        .Sum(st => st.MontoTerapeuta ?? 0);
                    var margen = servicio.MontoTotal - montoTerapeuta;
                    var comision = margen * 0.30m; // 30% para el presentador
                    comisionesDia += comision;
                }

                totalIngresosGenerados += montoDia;
                totalComisiones += comisionesDia;

                serviciosPorDia.Add(new ServiciosPorDiaDto
                {
                    Fecha = grupo.Key,
                    CantidadServicios = serviciosDia.Count,
                    MontoTotal = montoDia,
                    Comisiones = comisionesDia
                });
            }

            return new ReportePresentadorDto
            {
                PresentadorId = presentadorId,
                NombrePresentador = $"{presentador.Nombre} {presentador.Apellido}".Trim(),
                TotalIngresosGenerados = totalIngresosGenerados,
                TotalComisiones = totalComisiones,
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
            };
        }

        private async Task<ReportePresentadorDto> GenerarReportePresentador(int presentadorId,string nombre,string apellido,DateTime fechaInicio,DateTime fechaFin)
        {
            // Obtener servicios del presentador en el rango de fechas
            var servicios = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ComprobantesPago)
                .Where(s => s.PresentadorId == presentadorId &&
                           s.FechaServicio >= fechaInicio &&
                           s.FechaServicio <= fechaFin)
                .ToListAsync();

            // Calcular totales generales
            decimal totalIngresos = servicios.Sum(s => s.MontoTotal);

            // Obtener comisiones
            var comisiones = await _context.Comisiones
                .Where(c => c.PresentadorId == presentadorId &&
                           c.FechaCalculo >= fechaInicio &&
                           c.FechaCalculo <= fechaFin)
                .ToListAsync();

            decimal totalComisiones = comisiones.Sum(c => c.MontoComisionPresentador);

            // Calcular totales por tipo de pago
            var comprobantes = servicios
                .SelectMany(s => s.ServiciosTerapeutas
                    .SelectMany(st => st.ComprobantesPago))
                .Where(cp => cp.Estado == PagosEnum.EstadoComprobante.PAGADO)
                .ToList();

            decimal totalEfectivo = comprobantes
                .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.EFECTIVO)
                .Sum(cp => cp.Monto);

            decimal totalTransferencia = comprobantes
                .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.TRANSFERENCIA)
                .Sum(cp => cp.Monto);

            // Agrupar servicios por día
            var serviciosPorDia = servicios
                .GroupBy(s => s.FechaServicio.Date)
                .Select(g => new ServiciosPorDiaDto
                {
                    Fecha = g.Key,
                    CantidadServicios = g.Count(),
                    MontoTotal = g.Sum(s => s.MontoTotal),
                    Comisiones = comisiones
                        .Where(c => c.FechaCalculo.Date == g.Key)
                        .Sum(c => c.MontoComisionPresentador)
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            return new ReportePresentadorDto
            {
                PresentadorId = presentadorId,
                NombrePresentador = $"{nombre} {apellido}".Trim(),
                TotalIngresosGenerados = totalIngresos,
                TotalComisiones = totalComisiones,
                CantidadServicios = servicios.Count,
                TotalPagosEfectivo = totalEfectivo,
                TotalPagosTransferencia = totalTransferencia,
                ServiciosPorDia = serviciosPorDia
            };
        }

        public async Task<List<ReporteTerapeutaDto>> GetReporteTerapeutasAsync(ReporteTerapeutaFiltroDto filtro)
        {
            var query = _context.Terapeutas.AsQueryable();

            if (filtro.TerapeutaId.HasValue)
            {
                query = query.Where(t => t.Id == filtro.TerapeutaId.Value);
            }

            var terapeutas = await query.ToListAsync();
            var reportes = new List<ReporteTerapeutaDto>();

            foreach (var terapeuta in terapeutas)
            {
                var reporte = await GenerarReporteTerapeuta(
                    terapeuta.Id,
                    terapeuta.Nombre,
                    terapeuta.Apellido,
                    filtro.FechaInicio,
                    filtro.FechaFin);

                reportes.Add(reporte);
            }

            return reportes;
        }

        public async Task<ReporteTerapeutaDto> GetReporteTerapeutaAsync(int terapeutaId,DateTime fechaInicio,DateTime fechaFin)
        {
            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.Id == terapeutaId);

            if (terapeuta == null)
                throw new InvalidOperationException("Terapeuta no encontrado");

            // Obtener servicios liquidados o pagados
            var serviciosTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.ServiciosExtra)
                    .ThenInclude(se => se.ServicioExtraCatalogo)
                .Where(st => st.TerapeutaId == terapeutaId &&
                            st.Servicio.FechaServicio >= fechaInicio &&
                            st.Servicio.FechaServicio <= fechaFin &&
                            (st.Servicio.Estado == EstadosEnum.Servicio.LIQUIDADO ||
                             st.Servicio.Estado == EstadosEnum.Servicio.PAGADO))
                .OrderBy(st => st.Servicio.FechaServicio)
                .ToListAsync();

            // Agrupar por día
            var serviciosPorDia = serviciosTerapeuta
                .GroupBy(st => st.Servicio.FechaServicio.Date)
                .Select(grupo => {
                    var serviciosDia = grupo.ToList();

                    return new ServicioTerapeutaDiaDto
                    {
                        Fecha = grupo.Key,
                        CantidadServicios = serviciosDia.Count,
                        HorasTrabajadas = serviciosDia.Sum(st =>
                            st.HoraFin.HasValue && st.HoraInicio.HasValue ?
                            (int)(st.HoraFin.Value - st.HoraInicio.Value).TotalHours : 0),
                        MontoServiciosBase = serviciosDia.Sum(st => st.MontoTerapeuta ?? 0),
                        MontoServiciosExtra = serviciosDia.Sum(st =>
                            st.ServiciosExtra?.Sum(se => se.Monto) ?? 0)
                    };
                })
                .OrderBy(d => d.Fecha)
                .ToList();

            // Calcular totales
            var totalHorasTrabajadas = serviciosPorDia.Sum(d => d.HorasTrabajadas);
            var ingresosServiciosBase = serviciosPorDia.Sum(d => d.MontoServiciosBase);
            var ingresosServiciosExtra = serviciosPorDia.Sum(d => d.MontoServiciosExtra);

            return new ReporteTerapeutaDto
            {
                TerapeutaId = terapeutaId,
                NombreTerapeuta = $"{terapeuta.Nombre} {terapeuta.Apellido}".Trim(),
                TotalServicios = serviciosTerapeuta.Count,
                TotalHorasTrabajadas = totalHorasTrabajadas,
                IngresosServiciosBase = ingresosServiciosBase,
                IngresosServiciosExtra = ingresosServiciosExtra,
                TotalIngresos = ingresosServiciosBase + ingresosServiciosExtra,
                ServiciosPorDia = serviciosPorDia
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
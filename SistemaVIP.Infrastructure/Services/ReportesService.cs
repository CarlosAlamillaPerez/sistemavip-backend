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

        public async Task<ReportePresentadorDto> GetReportePresentadorAsync(
            int presentadorId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var presentador = await _context.Presentadores
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == presentadorId);

            if (presentador == null)
                throw new InvalidOperationException("Presentador no encontrado");

            return await GenerarReportePresentador(
                presentador.Id,
                presentador.Nombre,
                presentador.Apellido,
                fechaInicio,
                fechaFin);
        }

        private async Task<ReportePresentadorDto> GenerarReportePresentador(
            int presentadorId,
            string nombre,
            string apellido,
            DateTime fechaInicio,
            DateTime fechaFin)
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

        // ReportesService.cs - Agregar estos métodos
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

        public async Task<ReporteTerapeutaDto> GetReporteTerapeutaAsync(
            int terapeutaId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.Id == terapeutaId);

            if (terapeuta == null)
                throw new InvalidOperationException("Terapeuta no encontrada");

            return await GenerarReporteTerapeuta(
                terapeuta.Id,
                terapeuta.Nombre,
                terapeuta.Apellido,
                fechaInicio,
                fechaFin);
        }

        private async Task<ReporteTerapeutaDto> GenerarReporteTerapeuta(
            int terapeutaId,
            string nombre,
            string apellido,
            DateTime fechaInicio,
            DateTime fechaFin)
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

        public async Task<ReporteServiciosDto> GetReporteServiciosAsync(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            var servicios = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                .Where(s => s.FechaServicio >= fechaInicio &&
                            s.FechaServicio <= fechaFin)
                .ToListAsync();

            // Distribuición por tipo de ubicación
            var distribucion = new DistribucionServiciosDto
            {
                TotalConsultorio = servicios.Count(s => s.TipoUbicacion == ServicioEnum.TipoUbicacion.CONSULTORIO),
                TotalDomicilio = servicios.Count(s => s.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO),
                MontoConsultorio = servicios.Where(s => s.TipoUbicacion == ServicioEnum.TipoUbicacion.CONSULTORIO)
                                           .Sum(s => s.MontoTotal),
                MontoDomicilio = servicios.Where(s => s.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO)
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

            // Análisis de tipos de servicio (individual vs múltiple)
            var serviciosIndividuales = servicios.Where(s => s.ServiciosTerapeutas.Count == 1).ToList();
            var serviciosMultiples = servicios.Where(s => s.ServiciosTerapeutas.Count > 1).ToList();

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
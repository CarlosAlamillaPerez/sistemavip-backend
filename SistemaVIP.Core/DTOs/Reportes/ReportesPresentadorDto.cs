using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.DTOs.Reportes
{
    public class ReportePresentadorDto
    {
        public int PresentadorId { get; set; }
        public string NombrePresentador { get; set; }
        public decimal TotalIngresosGenerados { get; set; }
        public decimal MontoPorCancelaciones { get; set; }
        public decimal TotalComisiones { get; set; }
        public int CantidadServicios { get; set; }
        public decimal TotalPagosEfectivo { get; set; }
        public decimal TotalPagosTransferencia { get; set; }
        public List<ServiciosPorDiaDto> ServiciosPorDia { get; set; }
    }

    public class ServiciosPorDiaDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadServicios { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal Comisiones { get; set; }
    }

    public class ReportesPresentadorFiltroDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? PresentadorId { get; set; }
    }

    // En ReportePresentadorDto.cs, agregaremos:
    public class ReportePresentadorDetalladoDto
    {
        public int PresentadorId { get; set; }
        public string NombrePresentador { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal MontoPorCancelaciones { get; set; }
        public int CantidadServicios { get; set; }
        public decimal TotalPagosEfectivo { get; set; }
        public decimal TotalPagosTransferencia { get; set; }
        public List<ServicioPorEstadoDto> ServiciosPorEstado { get; set; }
        public List<ServicioPorDiaDetalladoDto> ServiciosPorDia { get; set; }
        public ResumenDiarioDto ResumenHoy { get; set; }
    }

    public class ServicioPorEstadoDto
    {
        public string Estado { get; set; }
        public int CantidadServicios { get; set; }
        public decimal MontoTotal { get; set; }
        public List<ServicioDetalladoDto> Servicios { get; set; }
    }

    public class ServicioExtraResumenDto
    {
        public string Nombre { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Notas { get; set; }
    }

    public class ComprobantePagoResumenDto
    {
        public string TipoComprobante { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? NumeroOperacion { get; set; }
    }

    public class ServicioDetalladoDto
    {
        public int ServicioId { get; set; }
        public DateTime FechaServicio { get; set; }
        public string TipoUbicacion { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; }

        public decimal? MontoCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
        public string? NotasCancelacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }


        public List<ServicioExtraResumenDto> ServiciosExtra { get; set; }
        public List<ComprobantePagoResumenDto> Comprobantes { get; set; }
    }

    public class ServicioPorDiaDetalladoDto : ServiciosPorDiaDto
    {
        public List<string> Estados { get; set; }
        public int ServiciosEnProceso { get; set; }
        public int ServiciosPendientes { get; set; }
        public int ServiciosFinalizados { get; set; }
        public decimal MontoServiciosExtra { get; set; }
    }

    public class ResumenDiarioDto
    {
        public int ServiciosActivos { get; set; }
        public int ServiciosPendientesVerificacion { get; set; }
        public decimal MontoTotalDia { get; set; }
        public decimal MontoServiciosExtra { get; set; }
        public List<ServicioDetalladoDto> ServiciosEnProceso { get; set; }
    }
}

using SistemaVIP.Core.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// DTOs/Reportes/ReporteTerapeutaDto.cs
public class ReporteTerapeutaDto
{
    public int TerapeutaId { get; set; }
    public string NombreTerapeuta { get; set; }
    public int TotalServicios { get; set; }
    public int TotalHorasTrabajadas { get; set; }
    public decimal IngresosServiciosBase { get; set; }
    public decimal IngresosServiciosExtra { get; set; }
    public decimal TotalIngresos { get; set; }
    public List<ServicioTerapeutaDiaDto> ServiciosPorDia { get; set; }
}

public class ServicioTerapeutaDiaDto
{
    public DateTime Fecha { get; set; }
    public int CantidadServicios { get; set; }
    public int HorasTrabajadas { get; set; }
    public decimal MontoServiciosBase { get; set; }
    public decimal MontoServiciosExtra { get; set; }
}

public class ReporteTerapeutaFiltroDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int? TerapeutaId { get; set; }
}



// Para GET /api/Reportes/terapeutas
public class ReporteTerapeutaResumenDto
{
    public int TerapeutaId { get; set; }
    public string NombreTerapeuta { get; set; }
    public int TotalServicios { get; set; }
    public decimal IngresosServiciosBase { get; set; }
    public decimal IngresosServiciosExtra { get; set; }
    public decimal TotalIngresos { get; set; }
    public int HorasTrabajadas { get; set; }
    public ProporcionServiciosDto ProporcionServicios { get; set; }
    public List<ServicioTerapeutaDiaDto> ServiciosPorDia { get; set; }
}

public class ProporcionServiciosDto
{
    public int ServiciosConsultorio { get; set; }
    public int ServiciosDomicilio { get; set; }
    public decimal IngresosConsultorio { get; set; }
    public decimal IngresosDomicilio { get; set; }
}

// Para GET /api/Reportes/terapeutas/{terapeutaId}
public class ReporteTerapeutaDetalladoDto
{
    public int TerapeutaId { get; set; }
    public string NombreTerapeuta { get; set; }
    public decimal TotalIngresos { get; set; }
    public decimal IngresosBase { get; set; }
    public decimal IngresosExtra { get; set; }
    public List<ServicioPorEstadoTerapeutaDto> ServiciosPorEstado { get; set; }
    public List<ServicioPorDiaTerapeutaDto> ServiciosPorDia { get; set; }
    public List<ServicioPorPresentadorDto> ServiciosPorPresentador { get; set; }
    public ResumenDiarioTerapeutaDto ResumenHoy { get; set; }
}

public class ServicioPorEstadoTerapeutaDto
{
    public string Estado { get; set; }
    public int CantidadServicios { get; set; }
    public decimal MontoBase { get; set; }
    public decimal MontoExtra { get; set; }
    public List<ServicioDetalladoTerapeutaDto> Servicios { get; set; }
}

public class ServicioDetalladoTerapeutaDto
{
    public int ServicioId { get; set; }
    public DateTime FechaServicio { get; set; }
    public string TipoUbicacion { get; set; }
    public string NombrePresentador { get; set; }
    public decimal MontoBase { get; set; }
    public string Estado { get; set; }
    public List<ServicioExtraResumenDto> ServiciosExtra { get; set; }
    public List<ComprobantePagoResumenDto> Comprobantes { get; set; }
    public DateTime? HoraInicio { get; set; }
    public DateTime? HoraFin { get; set; }
}

public class ServicioPorDiaTerapeutaDto
{
    public DateTime Fecha { get; set; }
    public int CantidadServicios { get; set; }
    public decimal MontoBase { get; set; }
    public decimal MontoExtra { get; set; }
    public int ServiciosEnProceso { get; set; }
    public int ServiciosPendientes { get; set; }
    public int ServiciosFinalizados { get; set; }
}

public class ServicioPorPresentadorDto
{
    public int PresentadorId { get; set; }
    public string NombrePresentador { get; set; }
    public int CantidadServicios { get; set; }
    public decimal MontoTotal { get; set; }
    public List<ServicioDetalladoTerapeutaDto> Servicios { get; set; }
}

public class ResumenDiarioTerapeutaDto
{
    public int ServiciosActivos { get; set; }
    public int ServiciosPendientesVerificacion { get; set; }
    public decimal MontoBaseHoy { get; set; }
    public decimal MontoExtrasHoy { get; set; }
    public List<ServicioDetalladoTerapeutaDto> ServiciosEnProceso { get; set; }
}
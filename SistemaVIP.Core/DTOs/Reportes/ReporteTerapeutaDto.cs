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
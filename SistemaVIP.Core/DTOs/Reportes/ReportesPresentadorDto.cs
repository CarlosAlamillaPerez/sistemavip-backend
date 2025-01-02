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

}

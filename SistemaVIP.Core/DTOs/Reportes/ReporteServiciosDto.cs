using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.DTOs.Reportes
{
    public class ReporteServiciosDto
    {
        public DistribucionServiciosDto Distribucion { get; set; }
        public List<HorarioServiciosDto> HorariosPopulares { get; set; }
        public TiposServicioDto TiposServicio { get; set; }
    }

    public class DistribucionServiciosDto
    {
        public int TotalConsultorio { get; set; }
        public int TotalDomicilio { get; set; }
        public decimal MontoConsultorio { get; set; }
        public decimal MontoDomicilio { get; set; }
    }

    public class HorarioServiciosDto
    {
        public int Hora { get; set; }
        public int CantidadServicios { get; set; }
        public decimal MontoPromedio { get; set; }
    }

    public class TiposServicioDto
    {
        public int ServiciosIndividuales { get; set; }
        public int ServiciosMultiples { get; set; }
        public decimal MontoServiciosIndividuales { get; set; }
        public decimal MontoServiciosMultiples { get; set; }
    }


}

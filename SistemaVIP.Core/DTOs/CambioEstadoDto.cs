using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.DTOs
{
    public class CambioEstadoDto
    {
        public string Estado { get; set; }
        public string? MotivoEstado { get; set; }

        // Información general
        public decimal MontoTotal { get; set; }
        public decimal MontoComprobantes { get; set; }

        // Información de comprobantes
        public List<ComprobantePagoResumenDto> Comprobantes { get; set; }

        // Información de liquidación
        public decimal MontoTerapeuta { get; set; }
        public decimal ComisionEmpresa { get; set; }
        public decimal ComisionPresentador { get; set; }
        public decimal TotalLiquidacion { get; set; }

        public CambioEstadoDto()
        {
            Comprobantes = new List<ComprobantePagoResumenDto>();
        }
    }

    public class ComprobantePagoResumenDto
    {
        public string TipoComprobante { get; set; }
        public decimal Monto { get; set; }
        public string NumeroOperacion { get; set; }
        public string Estado { get; set; }
    }

    public class CambioEstadoAsignacionDto
    {
        public int PresentadorId { get; set; }
        public int TerapeutaId { get; set; }
        public string Estado { get; set; }
    }
}
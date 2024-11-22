using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.DTOs
{
    public class ComisionResult
    {
        public decimal MontoTotal { get; set; }
        public decimal MontoTerapeuta { get; set; }
        public decimal MontoComisionTotal { get; set; }
        public decimal MontoComisionEmpresa { get; set; }
        public decimal MontoComisionPresentador { get; set; }
        public decimal PorcentajeAplicadoEmpresa { get; set; }
        public decimal PorcentajeAplicadoPresentador { get; set; }
    }
}

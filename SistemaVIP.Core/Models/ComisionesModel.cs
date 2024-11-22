using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class ComisionesModel
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int TerapeutaId { get; set; }
        public int PresentadorId { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoTerapeuta { get; set; }
        public decimal MontoComisionTotal { get; set; }
        public decimal MontoComisionEmpresa { get; set; }
        public decimal MontoComisionPresentador { get; set; }
        public decimal PorcentajeAplicadoEmpresa { get; set; }
        public decimal PorcentajeAplicadoPresentador { get; set; }
        public DateTime FechaCalculo { get; set; }
        public string Estado { get; set; }

        // Referencias a las entidades relacionadas
        public ServiciosModel Servicio { get; set; }
        public TerapeutaModel Terapeuta { get; set; }
        public PresentadorModel Presentador { get; set; }
    }
}
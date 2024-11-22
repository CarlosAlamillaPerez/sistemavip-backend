using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class ConfiguracionComisionesModel
    {
        public int Id { get; set; }
        public string TipoComision { get; set; }
        public decimal PorcentajeEmpresa { get; set; }
        public decimal PorcentajePresentador { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Referencia al usuario que creó la configuración
        public ApplicationUserModel User { get; set; }
    }
}
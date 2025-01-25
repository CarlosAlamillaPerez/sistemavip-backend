using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class PresentadorModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public decimal PorcentajeComision { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCambioEstado { get; set; }
        public string? MotivoEstado { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string? FotoUrl { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string? Notas { get; set; }

        // Referencia al usuario de Identity
        public virtual ApplicationUserModel User { get; set; }

        // Relación con TerapeutasPresentadores
        public virtual ICollection<TerapeutasPresentadoresModel> TerapeutasPresentadores { get; set; }

        public PresentadorModel()
        {
            TerapeutasPresentadores = new HashSet<TerapeutasPresentadoresModel>();
        }
    }
}
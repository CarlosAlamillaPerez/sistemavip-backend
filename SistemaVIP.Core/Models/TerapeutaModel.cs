using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class TerapeutaModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCambioEstado { get; set; }
        public string? MotivoEstado { get; set; }
        public string Estatura { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string? FotoUrl { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string? Notas { get; set; }
        public decimal TarifaBase { get; set; }
        public decimal TarifaExtra { get; set; }

        // Referencia al usuario de Identity
        public ApplicationUserModel? User { get; set; }

        public virtual ICollection<TerapeutasPresentadoresModel> TerapeutasPresentadores { get; set; }

        public TerapeutaModel()
        {
            TerapeutasPresentadores = new HashSet<TerapeutasPresentadoresModel>();
        }
    }
}
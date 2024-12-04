using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class ServiciosModel
    {
        public int Id { get; set; }
        public int PresentadorId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaServicio { get; set; }
        public string TipoServicio { get; set; }
        public string? Direccion { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
        public string? IdUsuarioCancelacion { get; set; }
        public string? NotasCancelacion { get; set; }
        public string? Notas { get; set; }

        // Referencias a las entidades relacionadas
        public PresentadorModel Presentador { get; set; }
        public ApplicationUserModel? UsuarioCancelacion { get; set; }
        public ICollection<ServiciosTerapeutasModel> ServiciosTerapeutas { get; set; }
    }
}
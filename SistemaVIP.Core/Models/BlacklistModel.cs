using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class BlacklistModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string IdUsuarioRegistro { get; set; }
        public string Estado { get; set; }
        public string? Notas { get; set; }

        // Referencia al usuario que registró
        public ApplicationUserModel UsuarioRegistro { get; set; }
    }
}
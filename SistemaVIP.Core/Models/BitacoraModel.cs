using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class BitacoraModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string IdUsuario { get; set; }
        public string Accion { get; set; }
        public string Tabla { get; set; }
        public string IdRegistro { get; set; }
        public string? ValoresAnteriores { get; set; }
        public string? ValoresNuevos { get; set; }

        // Referencia al usuario que realizó la acción
        public ApplicationUserModel Usuario { get; set; }
    }
}
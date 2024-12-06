using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class CatalogoEstadosModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TipoEntidad { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int OrdenVisualizacion { get; set; }
    }
}
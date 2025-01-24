using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class ServicioExtraCatalogoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }

        // Propiedad de navegación
        public virtual ICollection<ServicioExtraModel> ServiciosExtra { get; set; }
    }

    public class ServicioExtraModel
    {
        public int Id { get; set; }
        public int ServicioTerapeutaId { get; set; }
        public int ServicioExtraCatalogoId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Notas { get; set; }

        // Navegación
        public ServiciosTerapeutasModel ServicioTerapeuta { get; set; }
        public ServicioExtraCatalogoModel ServicioExtraCatalogo { get; set; }
    }
}

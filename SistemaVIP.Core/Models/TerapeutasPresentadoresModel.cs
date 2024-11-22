using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class TerapeutasPresentadoresModel
    {
        public int TerapeutaId { get; set; }
        public int PresentadorId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string Estado { get; set; }

        // Referencias a las entidades relacionadas
        public TerapeutaModel Terapeuta { get; set; }
        public PresentadorModel Presentador { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.DTOs
{
    public class CambioEstadoDto
    {
        public string Estado { get; set; }
        public string? MotivoEstado { get; set; }
    }

    public class CambioEstadoAsignacionDto
    {
        public int PresentadorId { get; set; }
        public int TerapeutaId { get; set; }
        public string Estado { get; set; }
    }
}
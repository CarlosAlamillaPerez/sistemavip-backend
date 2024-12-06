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
}
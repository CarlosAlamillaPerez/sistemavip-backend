using SistemaVIP.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IComisionService
    {
        ComisionResult CalcularComisiones(decimal montoTotal, decimal montoTerapeuta);
        Task<ComisionResult> CalcularComisionesAsync(decimal montoTotal, decimal montoTerapeuta);
        // Podemos agregar más métodos según necesitemos
    }
}

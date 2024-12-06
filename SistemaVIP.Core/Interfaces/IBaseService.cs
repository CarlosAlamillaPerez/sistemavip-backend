using SistemaVIP.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IBaseService<TDto>
    {
        Task<bool> CambiarEstadoAsync(int id, CambioEstadoDto cambioEstado);
    }
}


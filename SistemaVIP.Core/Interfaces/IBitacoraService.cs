using SistemaVIP.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IBitacoraService
    {
        Task<bool> RegistrarAccionAsync(string idUsuario, string accion, string tabla, string idRegistro, string valoresAnteriores = null, string valoresNuevos = null);

        Task<bool> RegistrarCambioEstadoAsync(string idUsuario, string tabla, string idRegistro, string estadoAnterior, string estadoNuevo, string motivo = null);

        Task<bool> RegistrarValidacionAsync(string idUsuario, string tabla, string idRegistro, bool exitosa, string detalles);

        Task<List<BitacoraDto>> GetByRegistroAsync(string tabla, string idRegistro);

        Task<List<BitacoraDto>> GetByFiltroAsync(DateTime fechaInicio, DateTime fechaFin, string tabla = null, string idUsuario = null);
    }
}
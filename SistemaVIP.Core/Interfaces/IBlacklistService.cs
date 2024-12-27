using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaVIP.Core.DTOs;

namespace SistemaVIP.Core.Interfaces
{
    public interface IBlacklistService
    {
        // Operaciones CRUD básicas
        Task<List<BlacklistDto>> GetAllAsync();
        Task<BlacklistDto> GetByIdAsync(int id);
        Task<BlacklistDto> CreateAsync(CreateBlacklistDto dto);
        Task<BlacklistDto> UpdateAsync(int id, UpdateBlacklistDto dto);
        Task<bool> DeleteAsync(int id); // Soft delete - cambia estado a inactivo

        // Búsquedas y filtros
        Task<List<BlacklistDto>> GetByFiltroAsync(BlacklistFiltroDto filtro);
        Task<List<BlacklistDto>> GetActivosAsync();

        // Verificaciones específicas
        Task<BlacklistVerificacionDto> VerificarTelefonoAsync(string telefono);
        Task<BlacklistVerificacionDto> VerificarEmailAsync(string email);
        Task<bool> ExisteRegistroActivoAsync(string telefono, string email);

        // Operaciones de estado
        Task<bool> CambiarEstadoAsync(int id, string nuevoEstado, string motivo = null);

        // Reportes y estadísticas
        Task<Dictionary<string, int>> GetEstadisticasRegistrosAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<BlacklistDto>> GetUltimosRegistrosAsync(int cantidad = 10);

        // Validaciones
        Task<bool> ValidarDatosRegistroAsync(CreateBlacklistDto dto);
        Task<(bool isValid, string errorMessage)> ValidarActualizacionAsync(int id, UpdateBlacklistDto dto);
    }
}
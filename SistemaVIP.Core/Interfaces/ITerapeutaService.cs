using SistemaVIP.Core.DTOs.Terapeuta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface ITerapeutaService
    {
        Task<List<TerapeutaDto>> GetAllAsync();
        Task<TerapeutaDto> GetByIdAsync(int id);
        Task<TerapeutaDto> GetByUserIdAsync(string userId);
        Task<TerapeutaDto> CreateAsync(CreateTerapeutaDto dto);
        Task<TerapeutaDto> UpdateAsync(int id, UpdateTerapeutaDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateEstadoAsync(int id, string estado);
        Task<List<TerapeutaDto>> GetActivosAsync();
        Task<bool> UpdateTarifasAsync(int id, decimal tarifaBase, decimal tarifaExtra);
    }
}
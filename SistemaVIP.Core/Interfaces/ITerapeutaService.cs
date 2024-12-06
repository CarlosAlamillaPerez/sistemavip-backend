using SistemaVIP.Core.DTOs.Terapeuta;
using SistemaVIP.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITerapeutaService : IBaseService<TerapeutaDto>
{
    Task<List<TerapeutaDto>> GetAllAsync();
    Task<TerapeutaDto> GetByIdAsync(int id);
    Task<TerapeutaDto> GetByUserIdAsync(string userId);
    Task<TerapeutaDto> CreateAsync(CreateTerapeutaDto dto);
    Task<TerapeutaDto> UpdateAsync(int id, UpdateTerapeutaDto dto);
    Task<List<TerapeutaDto>> GetActivosAsync();
    Task<bool> UpdateTarifasAsync(int id, decimal tarifaBase, decimal tarifaExtra);
    // Removido el método DeleteAsync
}
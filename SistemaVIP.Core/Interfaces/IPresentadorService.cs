using SistemaVIP.Core.DTOs.Presentador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IPresentadorService
    {
        Task<List<PresentadorDto>> GetAllAsync();
        Task<PresentadorDto> GetByIdAsync(int id);
        Task<PresentadorDto> GetByUserIdAsync(string userId);
        Task<PresentadorDto> CreateAsync(CreatePresentadorDto dto);
        Task<PresentadorDto> UpdateAsync(int id, UpdatePresentadorDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateEstadoAsync(int id, string estado);
        Task<List<PresentadorDto>> GetActivosAsync();
        Task<bool> UpdateComisionAsync(int id, decimal nuevoPorcentaje);
    }
}
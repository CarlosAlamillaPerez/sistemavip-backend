using SistemaVIP.Core.DTOs.Presentador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IPresentadorService : IBaseService<PresentadorDto>
    {
        Task<List<PresentadorDto>> GetAllAsync();
        Task<PresentadorDto> GetByIdAsync(int id);
        Task<PresentadorDto> GetByUserIdAsync(string userId);
        Task<PresentadorDto> CreateAsync(CreatePresentadorDto dto);
        Task<PresentadorDto> UpdateAsync(int id, UpdatePresentadorDto dto);
        Task<List<PresentadorDto>> GetActivosAsync();
        Task<bool> UpdateComisionAsync(int id, decimal nuevoPorcentaje);
        // Removido el método DeleteAsync
    }
}
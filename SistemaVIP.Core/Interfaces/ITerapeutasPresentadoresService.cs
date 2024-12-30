using SistemaVIP.Core.DTOs.TerapeutaPresentador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface ITerapeutasPresentadoresService
    {
        Task<List<TerapeutaPresentadorDto>> GetAllAsync();
        Task<List<TerapeutasPorPresentadorDto>> GetTerapeutasByPresentadorIdAsync(int presentadorId);
        Task<TerapeutaPresentadorDto> AsignarTerapeutaPresentadorAsync(AsignarTerapeutaPresentadorDto dto);
        Task<bool> UpdateEstadoAsync(int terapeutaId, int presentadorId, string estado);
        Task<bool> ExisteAsignacionAsync(int terapeutaId, int presentadorId);
        Task<bool> EliminarAsignacionAsync(int terapeutaId, int presentadorId);
    }
}
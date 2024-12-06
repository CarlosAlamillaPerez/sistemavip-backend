// Ubicación: SistemaVIP.Core/Interfaces/IValidacionesPresentadorService.cs
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IValidacionesPresentadorService
    {
        Task<(bool isValid, string errorMessage)> ValidarCambioEstado(int presentadorId, string nuevoEstado);
    }
}
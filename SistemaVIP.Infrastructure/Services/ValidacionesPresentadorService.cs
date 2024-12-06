// Ubicación: SistemaVIP.Infrastructure/Services/ValidacionesPresentadorService.cs
using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Infrastructure.Persistence.Context;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class ValidacionesPresentadorService : IValidacionesPresentadorService
    {
        private readonly ApplicationDbContext _context;

        public ValidacionesPresentadorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool isValid, string errorMessage)> ValidarCambioEstado(int presentadorId, string nuevoEstado)
        {
            // Validar servicios activos
            var tieneServiciosActivos = await _context.Servicios
                .AnyAsync(s => s.PresentadorId == presentadorId &&
                              (s.Estado == EstadosEnum.Servicio.PENDIENTE ||
                               s.Estado == EstadosEnum.Servicio.EN_PROCESO));

            if (tieneServiciosActivos)
            {
                return (false, "No se puede cambiar el estado del presentador porque tiene servicios activos pendientes.");
            }

            // Validar terapeutas activos asignados
            var tieneTerapeutasActivos = await _context.TerapeutasPresentadores
                .Include(tp => tp.Terapeuta)
                .AnyAsync(tp => tp.PresentadorId == presentadorId &&
                               tp.Estado == EstadosEnum.General.ACTIVO &&
                               tp.Terapeuta.Estado == EstadosEnum.General.ACTIVO);

            if (tieneTerapeutasActivos && nuevoEstado == EstadosEnum.General.INACTIVO)
            {
                return (false, "No se puede inactivar al presentador porque tiene terapeutas activos asignados.");
            }

            // Validar pagos o comisiones pendientes
            var tieneComisionesPendientes = await _context.Comisiones
                .AnyAsync(c => c.PresentadorId == presentadorId &&
                              c.Estado == "Pendiente");

            if (tieneComisionesPendientes)
            {
                return (false, "No se puede cambiar el estado del presentador porque tiene comisiones pendientes de pago.");
            }

            return (true, string.Empty);
        }
    }
}
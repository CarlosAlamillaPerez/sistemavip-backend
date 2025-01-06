using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IWhatsAppService
    {
        // Alertas de Servicio
        Task AlertarServicioExtendidoAsync(int servicioId, string motivo);
        Task NotificarInicioServicioAsync(int servicioId);
        Task NotificarFinServicioAsync(int servicioId);

        // Alertas de Ubicación
        Task AlertarAnomaliaUbicacionAsync(int servicioId, string tipoAnomalia, string detalles);
        Task AlertarServiciosSimultaneosAsync(int terapeutaId, int[] serviciosIds);
        Task AlertarDistanciaServiciosAsync(int terapeutaId, int servicio1Id, int servicio2Id, double distanciaKm);

        // Alertas de Pagos
        Task AlertarPagoPendienteAsync(int servicioId, int horasSinComprobante);
        Task AlertarMontoInusualAsync(int servicioId, decimal monto);
        Task AlertarErrorConciliacionAsync(int servicioId, string detallesError);

        // Alertas de Comportamiento
        Task AlertarConfirmacionesPendientesAsync(int terapeutaId, int cantidadPendiente);
        Task AlertarCancelacionesExcesivasAsync(int presentadorId, string mensaje);

        // Métodos de utilidad
        Task<bool> ValidarLimiteDiarioAsync();
        Task RegistrarEnvioMensajeAsync(string tipo, string destinatario, bool exitoso, string error = null);
        Task AlertarServicioCanceladoAsync(int servicioId, decimal? montoComision);

    }
}
using SistemaVIP.Core.Interfaces;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class NotificacionService : INotificacionService
    {
        // Aquí inyectarías la configuración del servicio de WhatsApp
        public async Task EnviarAlertaUbicacionAsync(
            string numeroTelefono,
            string nombreTerapeuta,
            double distanciaKm,
            string ubicacionInicial,
            string ubicacionFinal)
        {
            // Aquí implementarías la lógica de envío por WhatsApp
            var mensaje = $"⚠️ ALERTA DE UBICACIÓN\n" +
                         $"Terapeuta: {nombreTerapeuta}\n" +
                         $"Distancia de diferencia: {distanciaKm:F2} km\n" +
                         $"Ubicación inicial: {ubicacionInicial}\n" +
                         $"Ubicación final: {ubicacionFinal}";

            // Implementar lógica de envío de WhatsApp
            // Por ahora solo loggeamos el mensaje
            Console.WriteLine($"Enviando alerta WhatsApp a {numeroTelefono}: {mensaje}");
        }
    }
}
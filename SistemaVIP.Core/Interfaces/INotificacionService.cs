using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface INotificacionService
    {
        Task EnviarAlertaUbicacionAsync(string numeroTelefono, string nombreTerapeuta, double distanciaKm, string ubicacionInicial, string ubicacionFinal);
        // Podemos agregar más métodos de notificación según se necesite
    }
}
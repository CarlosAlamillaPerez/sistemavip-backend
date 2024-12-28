using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SistemaVIP.Core.Configuration;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;
using SistemaVIP.Core.Enums;

namespace SistemaVIP.Infrastructure.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppSettings _settings;
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private const string CALLMEBOT_API_URL = "https://api.callmebot.com/whatsapp.php";

        public WhatsAppService(
            IHttpClientFactory httpClientFactory,
            IOptions<WhatsAppSettings> settings,
            ApplicationDbContext context,
            IBitacoraService bitacoraService)
        {
            _httpClient = httpClientFactory.CreateClient("CallMeBot");
            _settings = settings.Value;
            _context = context;
            _bitacoraService = bitacoraService;
        }

        private async Task<bool> EnviarMensajeAsync(string mensaje, string phoneNumber = null)
        {
            if (!_settings.EnableNotifications)
                return true;

            if (!await ValidarLimiteDiarioAsync())
                return false;

            var destinatarios = phoneNumber != null
                ? new[] { phoneNumber }
                : _settings.AdminPhoneNumbers;

            var exito = true;
            var intentosRealizados = 0;

            while (intentosRealizados < _settings.MaxRetries)
            {
                try
                {
                    foreach (var numero in destinatarios)
                    {
                        var url = $"{CALLMEBOT_API_URL}?phone={numero}&apikey={_settings.ApiKey}&text={Uri.EscapeDataString(mensaje)}";
                        var response = await _httpClient.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            await RegistrarEnvioMensajeAsync("WhatsApp", numero, true);
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    intentosRealizados++;
                    if (intentosRealizados >= _settings.MaxRetries)
                    {
                        await RegistrarEnvioMensajeAsync("WhatsApp", string.Join(",", destinatarios), false, ex.Message);
                        exito = false;
                        break;
                    }
                    await Task.Delay(_settings.RetryDelayMilliseconds);
                }
            }

            return exito;
        }

        public async Task AlertarServicioExtendidoAsync(int servicioId, string motivo)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                return;

            var mensaje = $"⚠️ ALERTA: Servicio ⚠️\n" +
                         $"ID Servicio: {servicioId}\n" +
                         $"Presentador: {servicio.Presentador.Nombre} {servicio.Presentador.Apellido}\\n" +
                         $"Terapeuta: {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Nombre} {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Apellido}\n" +
                         $"Motivo: {motivo}";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task NotificarInicioServicioAsync(int servicioId)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                return;

            var horasTexto = servicio.DuracionHoras == 1 ? "hora" : "horas";
            var mensaje = $"🟢 INICIO DE SERVICIO\n" +
                          $"ID: {servicioId}\n" +
                          $"Presentador: {servicio.Presentador.Nombre} {servicio.Presentador.Apellido}\n" +
                          $"Terapeuta: {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Nombre} {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Apellido}\n" +
                          $"Tipo: {servicio.TipoUbicacion}\n" +
                          $"Monto: ${servicio.MontoTotal:N2}\n" +
                          $"Duración:{servicio.DuracionHoras} {horasTexto}";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task NotificarFinServicioAsync(int servicioId)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                return;

            var mensaje = $"🔴 FIN DE SERVICIO\n" +
                         $"ID: {servicioId}\n" +
                         $"Presentador: {servicio.Presentador.Nombre} {servicio.Presentador.Apellido}\n" +
                         $"Terapeuta: {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Nombre} {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Apellido}\n" +
                         $"Duración: {(servicio.ServiciosTerapeutas.FirstOrDefault()?.HoraFin - servicio.ServiciosTerapeutas.FirstOrDefault()?.HoraInicio)?.TotalHours:F1} horas";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task AlertarAnomaliaUbicacionAsync(int servicioId, string tipoAnomalia, string detalles)
        {
            var servicio = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                return;

            var mensaje = $"⚠️ ALERTA DE UBICACIÓN ⚠️\n" +
                         $"Servicio: {servicioId}\n" +
                         $"Tipo: {tipoAnomalia}\n" +
                         $"Presentador: {servicio.Presentador.Nombre} {servicio.Presentador.Apellido}\n" +
                         $"Terapeuta: {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Nombre} {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Apellido}\n" +
                         $"Detalles: {detalles}";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task AlertarServiciosSimultaneosAsync(int terapeutaId, int[] serviciosIds)
        {
            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.Id == terapeutaId);

            if (terapeuta == null)
                return;

            var mensaje = $"⚠️ ALERTA: SERVICIOS SIMULTÁNEOS\n" +
                         $"Terapeuta: {terapeuta.Nombre}\n" +
                         $"IDs Servicios: {string.Join(", ", serviciosIds)}\n" +
                         $"Cantidad: {serviciosIds.Length} servicios";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task AlertarDistanciaServiciosAsync(int terapeutaId, int servicio1Id, int servicio2Id, double distanciaKm)
        {
            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.Id == terapeutaId);

            if (terapeuta == null)
                return;

            var mensaje = $"⚠️ ALERTA: DISTANCIA ENTRE SERVICIOS\n" +
                         $"Terapeuta: {terapeuta.Nombre}\n" +
                         $"Servicio 1: {servicio1Id}\n" +
                         $"Servicio 2: {servicio2Id}\n" +
                         $"Distancia: {distanciaKm:F1} km";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task AlertarPagoPendienteAsync(int servicioId, int horasSinComprobante)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Presentador)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                return;

            var mensaje = $"💰 ALERTA DE PAGO PENDIENTE\n" +
                         $"Servicio: {servicioId}\n" +
                         $"Presentador: {servicio.Presentador.Nombre} {servicio.Presentador.Apellido}\n" +
                         $"Horas sin comprobante: {horasSinComprobante}\n" +
                         $"Monto: ${servicio.MontoTotal:N2}";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task AlertarMontoInusualAsync(int servicioId, decimal montoPorHora)
        {
            // Primero verificamos si ya se ha enviado una alerta para este servicio
            var yaSeNotificoBajoMonto = await _context.Bitacora
                .AnyAsync(b => b.Tabla == BitacoraEnum.TablaMonitoreo.SERVICIOS &&
                              b.IdRegistro == servicioId.ToString() &&
                              b.Accion == "ALERTA_MONTO_BAJO");

            if (yaSeNotificoBajoMonto)
                return;

            var servicio = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ComprobantesPago)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                return;

            // Calcular el monto total de todos los comprobantes
            var montoTotalComprobantes = servicio.ServiciosTerapeutas
                .SelectMany(st => st.ComprobantesPago)
                .Where(cp => cp.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE &&
                             cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .Sum(cp => cp.Monto);

            // Solo alertar si ya hay al menos un comprobante y el monto por hora es bajo
            if (montoTotalComprobantes > 0)
            {
                var montoPorHoraReal = montoTotalComprobantes / servicio.DuracionHoras;
                const decimal MONTO_MINIMO_POR_HORA = 1500;

                if (montoPorHoraReal < MONTO_MINIMO_POR_HORA)
                {
                    var mensaje = $"⚠️ ALERTA: Monto por hora bajo\n" +
                                 $"ID Servicio: {servicio.Id}\n" +
                                 $"Presentador: {servicio.Presentador.Nombre} {servicio.Presentador.Apellido}\n" +
                                 $"Terapeuta: {servicio.ServiciosTerapeutas.FirstOrDefault()?.Terapeuta.Nombre}\n" +
                                 $"Monto por hora: ${montoPorHoraReal:N2}\n" +
                                 $"Horas: {servicio.DuracionHoras}\n" +
                                 $"Monto total actual: ${montoTotalComprobantes:N2}\n" +
                                 $"Monto total esperado: ${servicio.MontoTotal:N2}";

                    await EnviarMensajeAsync(mensaje);

                    // Registrar que ya se envió la alerta
                    await _bitacoraService.RegistrarAccionAsync(
                        "SYSTEM",
                        "ALERTA_MONTO_BAJO",
                        BitacoraEnum.TablaMonitoreo.SERVICIOS,
                        servicioId.ToString(),
                        null,
                        JsonSerializer.Serialize(new
                        {
                            MontoPorHora = montoPorHoraReal,
                            MontoTotalActual = montoTotalComprobantes,
                            MontoTotalEsperado = servicio.MontoTotal
                        })
                    );
                }
            }
        }

        public async Task AlertarErrorConciliacionAsync(int servicioId, string detallesError)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Presentador)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                return;

            var mensaje = $"⚠️ ERROR DE CONCILIACIÓN\n" +
                         $"Servicio: {servicioId}\n" +
                         $"Presentador: {servicio.Presentador.Nombre}\n" +
                         $"Error: {detallesError}";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task AlertarConfirmacionesPendientesAsync(int terapeutaId, int cantidadPendiente)
        {
            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.Id == terapeutaId);

            if (terapeuta == null)
                return;

            var mensaje = $"⚠️ CONFIRMACIONES PENDIENTES\n" +
                         $"Terapeuta: {terapeuta.Nombre}\n" +
                         $"Pendientes: {cantidadPendiente}";

            await EnviarMensajeAsync(mensaje);
        }

        public async Task AlertarCancelacionesExcesivasAsync(int presentadorId, string mensaje)
        {
            try
            {
                var resultado = await EnviarMensajeAsync(mensaje);

                // Registrar en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    "SYSTEM",
                    BitacoraEnum.TipoAccion.VALIDACION,
                    BitacoraEnum.TablaMonitoreo.PRESENTADORES,
                    presentadorId.ToString(),
                    null,
                    JsonSerializer.Serialize(new
                    {
                        TipoAlerta = "CancelacionesExcesivas",
                        Mensaje = mensaje,
                        ResultadoEnvio = resultado
                    })
                );
            }
            catch (Exception ex)
            {
                // Registrar error en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    "SYSTEM",
                    "ERROR_NOTIFICACION",
                    BitacoraEnum.TablaMonitoreo.PRESENTADORES,
                    presentadorId.ToString(),
                    null,
                    JsonSerializer.Serialize(new { Error = ex.Message })
                );
            }
        }

        public async Task<bool> ValidarLimiteDiarioAsync()
        {
            // Implementar lógica de verificación de límite diario
            // usando la tabla de bitácora para contar mensajes enviados hoy
            var hoy = DateTime.UtcNow.Date;
            var mensajesHoy = await _context.Bitacora
                .CountAsync(b => b.Fecha.Date == hoy &&
                               b.Tabla == "WhatsAppMessages" &&
                               b.Accion == "ENVIO_MENSAJE" &&
                               b.ValoresNuevos.Contains("\"exitoso\":true"));

            return mensajesHoy < _settings.DailyMessageLimit;
        }

        public async Task RegistrarEnvioMensajeAsync(string tipo, string destinatario, bool exitoso, string error = null)
        {
            var datos = new
            {
                tipo,
                destinatario,
                exitoso,
                error,
                fecha = DateTime.UtcNow
            };

            await _bitacoraService.RegistrarAccionAsync(
                "SYSTEM",
                "ENVIO_MENSAJE",
                "WhatsAppMessages",
                Guid.NewGuid().ToString(),
                null,
                JsonSerializer.Serialize(datos)
            );
        }
    }
}
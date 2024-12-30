using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Servicio;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;

namespace SistemaVIP.Infrastructure.Services
{
    public class ServicioService : IServicioService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocationService _locationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly GeometryFactory _geometryFactory;
        private readonly IBitacoraService _bitacoraService;
        private readonly IWhatsAppService _whatsAppService;

        public ServicioService(
            ApplicationDbContext context,
            ILocationService locationService,
            ICurrentUserService currentUserService,
            IBitacoraService bitacoraService,
            IWhatsAppService whatsAppService)
        {
            _context = context;
            _locationService = locationService;
            _currentUserService = currentUserService;
            _bitacoraService = bitacoraService;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            _whatsAppService = whatsAppService;
        }

        public async Task<List<ServicioDto>> GetAllAsync()
        {
            var servicios = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.ComprobantesPago)
                        .ThenInclude(cp => cp.UsuarioRegistro) 
                .OrderByDescending(s => s.FechaServicio)
                .ToListAsync();

            return servicios.Select(MapToDto).ToList();
        }

        public async Task<ServicioDto> GetByIdAsync(int id)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .FirstOrDefaultAsync(s => s.Id == id);

            return servicio == null ? null : MapToDto(servicio);
        }

        public async Task<ServicioDto> CreateAsync(CreateServicioDto dto)
        {
            // Validar el servicio
            await ValidateServicioCreation(dto);

            var servicio = new ServiciosModel
            {
                PresentadorId = dto.PresentadorId,
                FechaSolicitud = DateTime.Now,
                FechaServicio = dto.FechaServicio,
                TipoUbicacion = dto.TipoUbicacion, 
                Direccion = dto.Direccion,
                MontoTotal = dto.MontoTotal,
                GastosTransporte = dto.GastosTransporte,
                NotasTransporte = dto.NotasTransporte,
                Estado = EstadosEnum.Servicio.PENDIENTE,
                Notas = dto.Notas,
                DuracionHoras = dto.DuracionHoras
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            // Registrar en bitácora la creación del servicio
            await _bitacoraService.RegistrarAccionAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TipoAccion.CREACION,
                BitacoraEnum.TablaMonitoreo.SERVICIOS,
                servicio.Id.ToString(),
                null,
                JsonSerializer.Serialize(new
                {
                    PresentadorId = dto.PresentadorId,
                    FechaServicio = dto.FechaServicio,
                    TipoUbicacion = dto.TipoUbicacion,
                    MontoTotal = dto.MontoTotal,
                    CantidadTerapeutas = dto.Terapeutas.Count
                })
            );

            foreach (var terapeutaDto in dto.Terapeutas)
            {
                var servicioTerapeuta = new ServiciosTerapeutasModel
                {
                    ServicioId = servicio.Id,
                    TerapeutaId = terapeutaDto.TerapeutaId,
                    FechaAsignacion = DateTime.Now,
                    MontoTerapeuta = terapeutaDto.MontoTerapeuta,
                    Estado = EstadosEnum.Servicio.PENDIENTE,
                    LinkConfirmacion = Guid.NewGuid(),
                    LinkFinalizacion = Guid.NewGuid()
                };

                _context.ServiciosTerapeutas.Add(servicioTerapeuta);

                // Registrar en bitácora cada asignación de terapeuta
                await _bitacoraService.RegistrarAccionAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TipoAccion.CREACION,
                    BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                    $"{servicio.Id}-{terapeutaDto.TerapeutaId}",
                    null,
                    JsonSerializer.Serialize(new
                    {
                        ServicioId = servicio.Id,
                        TerapeutaId = terapeutaDto.TerapeutaId,
                        MontoTerapeuta = terapeutaDto.MontoTerapeuta
                    })
                );
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(servicio.Id);
        }

        public async Task<ServicioDto> UpdateAsync(int id, UpdateServicioDto updateDto)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null) return null;

            if (updateDto.FechaServicio.HasValue)
                servicio.FechaServicio = updateDto.FechaServicio.Value;
            if (!string.IsNullOrEmpty(updateDto.Direccion))
                servicio.Direccion = updateDto.Direccion;
            if (updateDto.MontoTotal.HasValue)
                servicio.MontoTotal = updateDto.MontoTotal.Value;
            if (!string.IsNullOrEmpty(updateDto.Notas))
                servicio.Notas = updateDto.Notas;
            if (updateDto.DuracionHoras.HasValue)
            {
                if (updateDto.DuracionHoras <= 0 || updateDto.DuracionHoras > 24)
                {
                    throw new InvalidOperationException("La duración del servicio debe ser entre 1 y 24 horas");
                }
                servicio.DuracionHoras = updateDto.DuracionHoras.Value;
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<ServicioDto> CancelarServicioAsync(int id, CancelacionServicioDto cancelacionDto, string userId)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null) return null;

            servicio.Estado = "Cancelado";
            servicio.FechaCancelacion = DateTime.Now;
            servicio.MotivoCancelacion = cancelacionDto.MotivoCancelacion;
            servicio.NotasCancelacion = cancelacionDto.NotasCancelacion;
            servicio.IdUsuarioCancelacion = userId;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<ServicioTerapeutaDto> GetServicioTerapeutaByLinkConfirmacionAsync(Guid linkConfirmacion)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.Terapeuta)
                .FirstOrDefaultAsync(st => st.LinkConfirmacion == linkConfirmacion);

            return servicioTerapeuta == null ? null : MapToServicioTerapeutaDto(servicioTerapeuta);
        }

        public async Task<ServicioTerapeutaDto> GetServicioTerapeutaByLinkFinalizacionAsync(Guid linkFinalizacion)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.Terapeuta)
                .FirstOrDefaultAsync(st => st.LinkFinalizacion == linkFinalizacion);

            return servicioTerapeuta == null ? null : MapToServicioTerapeutaDto(servicioTerapeuta);
        }

        public async Task<List<ServicioDto>> GetServiciosByPresentadorAsync(int presentadorId)
        {
            var servicios = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .Where(s => s.PresentadorId == presentadorId)
                .OrderByDescending(s => s.FechaServicio)
                .ToListAsync();

            return servicios.Select(MapToDto).ToList();
        }

        public async Task<List<ServicioDto>> GetServiciosByTerapeutaAsync(int terapeutaId)
        {
            var servicios = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .Where(s => s.ServiciosTerapeutas.Any(st => st.TerapeutaId == terapeutaId))
                .OrderByDescending(s => s.FechaServicio)
                .ToListAsync();

            return servicios.Select(MapToDto).ToList();
        }

        public async Task<List<ServicioDto>> GetServiciosByFechaAsync(DateTime fecha)
        {
            var servicios = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .Where(s => s.FechaServicio.Date == fecha.Date)
                .OrderByDescending(s => s.FechaServicio)
                .ToListAsync();

            return servicios.Select(MapToDto).ToList();
        }

        public async Task<List<ServicioDto>> GetServiciosActivosAsync()
        {
            var servicios = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
                .Where(s => s.Estado == "Pendiente" || s.Estado == "EnProceso")
                .OrderByDescending(s => s.FechaServicio)
                .ToListAsync();

            return servicios.Select(MapToDto).ToList();
        }

        public async Task<bool> ValidateServicioTerapeutasAsync(List<CreateServicioTerapeutaDto> terapeutas, int presentadorId)
        {
            // Primero verificar que el presentador esté activo
            var presentador = await _context.Presentadores
                .FirstOrDefaultAsync(p => p.Id == presentadorId);

            if (presentador == null || presentador.Estado != EstadosEnum.General.ACTIVO)
                throw new InvalidOperationException("El presentador no está activo o no existe");

            foreach (var terapeuta in terapeutas)
            {
                // Verificar que el terapeuta esté activo
                var terapeutaEntity = await _context.Terapeutas
                    .FirstOrDefaultAsync(t => t.Id == terapeuta.TerapeutaId);

                if (terapeutaEntity == null || terapeutaEntity.Estado != EstadosEnum.General.ACTIVO)
                    throw new InvalidOperationException($"El terapeuta con ID {terapeuta.TerapeutaId} no está activo o no existe");

                // Verificar la asignación al presentador
                var asignacion = await _context.TerapeutasPresentadores
                    .FirstOrDefaultAsync(tp =>
                        tp.TerapeutaId == terapeuta.TerapeutaId &&
                        tp.PresentadorId == presentadorId &&
                        tp.Estado == EstadosEnum.General.ACTIVO);

                if (asignacion == null)
                    throw new InvalidOperationException($"El terapeuta con ID {terapeuta.TerapeutaId} no está asignado al presentador");

                // Verificar si el terapeuta ya tiene un servicio activo en el mismo horario
                var serviciosActivos = await _context.ServiciosTerapeutas
                    .Include(st => st.Servicio)
                    .Where(st =>
                        st.TerapeutaId == terapeuta.TerapeutaId &&
                        st.Estado != EstadosEnum.Servicio.FINALIZADO &&
                        st.Estado != EstadosEnum.Servicio.CANCELADO)
                    .ToListAsync();

                if (serviciosActivos.Any())
                    throw new InvalidOperationException($"El terapeuta con ID {terapeuta.TerapeutaId} ya tiene servicios activos");
            }

            return true;
        }

        public async Task<bool> ValidateLocationDistanceAsync(double startLat, double startLng, double endLat, double endLng)
        {
            return await Task.FromResult(_locationService.IsWithinSameArea(startLat, startLng, endLat, endLng));
        }

        private ServicioDto MapToDto(ServiciosModel servicio)
        {
            return new ServicioDto
            {
                Id = servicio.Id,
                PresentadorId = servicio.PresentadorId,
                FechaSolicitud = servicio.FechaSolicitud,
                FechaServicio = servicio.FechaServicio,
                TipoUbicacion = servicio.TipoUbicacion,
                Direccion = servicio.Direccion,
                MontoTotal = servicio.MontoTotal,
                GastosTransporte = servicio.GastosTransporte,
                NotasTransporte = servicio.NotasTransporte,
                Estado = servicio.Estado,
                FechaCancelacion = servicio.FechaCancelacion,
                MotivoCancelacion = servicio.MotivoCancelacion,
                NotasCancelacion = servicio.NotasCancelacion,
                Notas = servicio.Notas,
                NombrePresentador = $"{servicio.Presentador?.Nombre} {servicio.Presentador?.Apellido}".Trim(),
                DuracionHoras = servicio.DuracionHoras,
                Terapeutas = servicio.ServiciosTerapeutas?
                    .Select(st => new ServicioTerapeutaDto
                    {
                        Id = st.Id,
                        ServicioId = st.ServicioId,
                        TerapeutaId = st.TerapeutaId,
                        NombreTerapeuta = $"{st.Terapeuta?.Nombre} {st.Terapeuta?.Apellido}".Trim(),
                        FechaAsignacion = st.FechaAsignacion,
                        HoraInicio = st.HoraInicio,
                        HoraFin = st.HoraFin,
                        Estado = st.Estado,
                        MontoTerapeuta = st.MontoTerapeuta,
                        MontoEfectivo = st.MontoEfectivo,
                        MontoTransferencia = st.MontoTransferencia,
                        LinkConfirmacion = st.LinkConfirmacion,
                        LinkFinalizacion = st.LinkFinalizacion,
                        ComprobantesPago = st.ComprobantesPago?
                            .Select(cp => new ComprobantePagoDto
                            {
                                Id = cp.Id,
                                ServicioTerapeutaId = cp.ServicioTerapeutaId,
                                TipoComprobante = cp.TipoComprobante,
                                OrigenPago = cp.OrigenPago,
                                NumeroOperacion = cp.NumeroOperacion,
                                UrlComprobante = cp.UrlComprobante,
                                FechaRegistro = cp.FechaRegistro,
                                Estado = cp.Estado,
                                NotasComprobante = cp.NotasComprobante,
                                NombreUsuarioRegistro = cp.UsuarioRegistro?.UserName ?? "Sistema",
                                Monto = cp.Monto,
                                MotivoRechazo = cp.MotivoRechazo
                            }).ToList() ?? new List<ComprobantePagoDto>()
                    })
                    .ToList() ?? new List<ServicioTerapeutaDto>()
            };
        }

        private ServicioTerapeutaDto MapToServicioTerapeutaDto(ServiciosTerapeutasModel st)
        {
            return new ServicioTerapeutaDto
            {
                Id = st.Id,  // Nuevo campo
                ServicioId = st.ServicioId,
                TerapeutaId = st.TerapeutaId,
                NombreTerapeuta = $"{st.Terapeuta?.Nombre} {st.Terapeuta?.Apellido}".Trim(),
                FechaAsignacion = st.FechaAsignacion,
                HoraInicio = st.HoraInicio,
                HoraFin = st.HoraFin,
                Estado = st.Estado,
                MontoTerapeuta = st.MontoTerapeuta,
                MontoEfectivo = st.MontoEfectivo,
                MontoTransferencia = st.MontoTransferencia,
                LinkConfirmacion = st.LinkConfirmacion,
                LinkFinalizacion = st.LinkFinalizacion,
                ComprobantesPago = st.ComprobantesPago?.Select(cp => new ComprobantePagoDto
                {
                    Id = cp.Id,
                    ServicioTerapeutaId = cp.ServicioTerapeutaId,
                    TipoComprobante = cp.TipoComprobante,
                    OrigenPago = cp.OrigenPago,
                    NumeroOperacion = cp.NumeroOperacion,
                    UrlComprobante = cp.UrlComprobante,
                    FechaRegistro = cp.FechaRegistro,
                    Estado = cp.Estado,
                    NotasComprobante = cp.NotasComprobante,
                    NombreUsuarioRegistro = cp.UsuarioRegistro?.UserName ?? "Sistema",
                    Monto = cp.Monto
                }).ToList() ?? new List<ComprobantePagoDto>()
            };
        }

        public async Task<ServicioTerapeutaDto> ConfirmarInicioServicioAsync(ConfirmacionServicioDto confirmacionDto)
        {

            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Terapeuta)
                .FirstOrDefaultAsync(st => st.LinkConfirmacion == confirmacionDto.LinkConfirmacion);

            if (servicioTerapeuta == null) return null;

            var estadoAnterior = servicioTerapeuta.Estado;
            var horaInicio = DateTime.Now;

            servicioTerapeuta.HoraInicio = horaInicio;
            servicioTerapeuta.Estado = EstadosEnum.Servicio.EN_PROCESO;
            servicioTerapeuta.UbicacionInicio = _geometryFactory.CreatePoint(
                new Coordinate(confirmacionDto.Longitud, confirmacionDto.Latitud));

            // Validar servicios simultáneos
            await ValidarServiciosSimultaneosAsync(
                servicioTerapeuta.TerapeutaId,
                horaInicio,
                horaInicio.AddHours(2) // Asumimos 2 horas de duración estándar
            );

            // Registrar cambio de estado
            await _bitacoraService.RegistrarCambioEstadoAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                $"{servicioTerapeuta.ServicioId}-{servicioTerapeuta.TerapeutaId}",
                estadoAnterior,
                EstadosEnum.Servicio.EN_PROCESO,
                "Confirmación inicio de servicio"
            );

            decimal montoMinimoPorHora = 1500;
            decimal montoPorHora = servicioTerapeuta.Servicio.MontoTotal / servicioTerapeuta.Servicio.DuracionHoras;

            if (montoPorHora < montoMinimoPorHora)
            {
                await _whatsAppService.AlertarMontoInusualAsync(
                    servicioTerapeuta.ServicioId,
                    montoPorHora);
            }

            // Notificar inicio del servicio
            await _whatsAppService.NotificarInicioServicioAsync(servicioTerapeuta.ServicioId);

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByLinkConfirmacionAsync(confirmacionDto.LinkConfirmacion);
        }

        public async Task<ServicioTerapeutaDto> FinalizarServicioAsync(FinalizacionServicioDto finalizacionDto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Terapeuta)
                .Include(st => st.Servicio.Presentador)
                .FirstOrDefaultAsync(st => st.LinkFinalizacion == finalizacionDto.LinkFinalizacion);

            if (servicioTerapeuta == null)
                return null;

            var estadoAnterior = servicioTerapeuta.Estado;
            var ubicacionAnterior = servicioTerapeuta.UbicacionFin;
            var horaFin = DateTime.Now;

            // Validaciones de finalización
            await ValidarFinalizacionServicio(
                servicioTerapeuta,
                horaFin,
                finalizacionDto.Latitud,
                finalizacionDto.Longitud
            );

            // Actualizar servicio
            servicioTerapeuta.HoraFin = horaFin;
            servicioTerapeuta.Estado = EstadosEnum.Servicio.FINALIZADO;
            servicioTerapeuta.UbicacionFin = _geometryFactory.CreatePoint(
                new Coordinate(finalizacionDto.Longitud, finalizacionDto.Latitud));

            // Registrar cambio de estado
            await _bitacoraService.RegistrarCambioEstadoAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                $"{servicioTerapeuta.ServicioId}-{servicioTerapeuta.TerapeutaId}",
                estadoAnterior,
                EstadosEnum.Servicio.FINALIZADO,
                $"Finalización de servicio. Duración: {(horaFin - servicioTerapeuta.HoraInicio.Value).TotalHours:F2} horas"
            );

            // Registrar detalles de la finalización
            await _bitacoraService.RegistrarAccionAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TipoAccion.VALIDACION_UBICACION,
                BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                $"{servicioTerapeuta.ServicioId}-{servicioTerapeuta.TerapeutaId}",
                ubicacionAnterior?.ToString(),
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    Ubicacion = new { Latitud = finalizacionDto.Latitud, Longitud = finalizacionDto.Longitud },
                    HoraFin = servicioTerapeuta.HoraFin,
                    DuracionHoras = (horaFin - servicioTerapeuta.HoraInicio.Value).TotalHours
                })
            );

            // Notificar finalización del servicio
            await _whatsAppService.NotificarFinServicioAsync(servicioTerapeuta.ServicioId);

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByLinkFinalizacionAsync(finalizacionDto.LinkFinalizacion);
        }

        private async Task<bool> ValidarNumeroOperacionUnico(string numeroOperacion)
        {
            if (string.IsNullOrEmpty(numeroOperacion))
                return true;

            return !await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                .AnyAsync(st => st.ComprobantesPago
                    .Any(cp => cp.NumeroOperacion == numeroOperacion));
        }

        public async Task<ServicioTerapeutaDto> AgregarComprobantePagoAsync(int servicioTerapeutaId, CreateComprobantePagoDto dto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Servicio)
                .Include(st => st.Terapeuta)
                .Include(st => st.Servicio.Presentador)
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            // Validar el nuevo comprobante
            await ValidarComprobantePago(servicioTerapeuta, dto);
            await ValidarComprobante(servicioTerapeutaId, dto); // Mantener validaciones existentes

            var estadoAnterior = servicioTerapeuta.Estado;

            var comprobante = new ComprobantePagoModel
            {
                ServicioTerapeutaId = servicioTerapeutaId,
                TipoComprobante = dto.TipoComprobante,
                OrigenPago = dto.OrigenPago,
                NumeroOperacion = dto.NumeroOperacion,
                UrlComprobante = dto.UrlComprobante,
                NotasComprobante = dto.NotasComprobante,
                Estado = PagosEnum.EstadoComprobante.PENDIENTE,
                FechaRegistro = DateTime.Now,
                IdUsuarioRegistro = _currentUserService.GetUserId(),
                Monto = dto.Monto
            };

            servicioTerapeuta.ComprobantesPago.Add(comprobante);

            // Registrar la creación del comprobante
            await _bitacoraService.RegistrarAccionAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TipoAccion.REGISTRO_PAGO,
                BitacoraEnum.TablaMonitoreo.COMPROBANTES_PAGO,
                comprobante.Id.ToString(),
                null,
                JsonSerializer.Serialize(new
                {
                    dto.TipoComprobante,
                    dto.OrigenPago,
                    dto.NumeroOperacion,
                    dto.Monto,
                    EstadoInicial = PagosEnum.EstadoComprobante.PENDIENTE
                })
            );

            // Si es el primer comprobante, actualizar estado del servicio
            if (servicioTerapeuta.Estado == EstadosEnum.Servicio.FINALIZADO)
            {
                servicioTerapeuta.Estado = EstadosEnum.Servicio.POR_CONFIRMAR;

                // Registrar cambio de estado del servicio
                await _bitacoraService.RegistrarCambioEstadoAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                    servicioTerapeuta.Id.ToString(),
                    estadoAnterior,
                    EstadosEnum.Servicio.POR_CONFIRMAR,
                    "Registro de primer comprobante de pago"
                );
            }

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByIdAsync(servicioTerapeutaId);
        }

        public async Task<ServicioTerapeutaDto> ActualizarEstadoComprobanteAsync(int servicioTerapeutaId,int comprobanteId,string nuevoEstado)
        {
            if (!PagosEnum.EstadosComprobante.Contains(nuevoEstado))
                throw new InvalidOperationException("Estado no válido");

            var comprobante = await _context.ComprobantesPago
                .FirstOrDefaultAsync(cp => cp.Id == comprobanteId &&
                                         cp.ServicioTerapeutaId == servicioTerapeutaId);

            if (comprobante == null)
                throw new InvalidOperationException("Comprobante no encontrado");

            comprobante.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return await GetServicioTerapeutaByIdAsync(servicioTerapeutaId);
        }

        private async Task<ServicioTerapeutaDto> GetServicioTerapeutaByIdAsync(int id)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                    .ThenInclude(cp => cp.UsuarioRegistro)
                .Include(st => st.Terapeuta)
                .FirstOrDefaultAsync(st => st.Id == id);

            return servicioTerapeuta != null ? MapToServicioTerapeutaDto(servicioTerapeuta) : null;
        }

        private async Task ValidateServicioCreation(CreateServicioDto dto)
        {
            // Validar tipo de ubicación
            if (!ServicioEnum.TiposUbicacion.Contains(dto.TipoUbicacion))
            {
                throw new InvalidOperationException("Tipo de ubicación no válido");
            }

            // Validar duración del servicio
            if (dto.DuracionHoras <= 0 || dto.DuracionHoras > 24)
            {
                throw new InvalidOperationException("La duración del servicio debe ser entre 1 y 24 horas");
            }

            // Validar monto total
            if (dto.MontoTotal <= 0)
            {
                throw new InvalidOperationException("El monto total debe ser mayor que cero");
            }

            // Validar montos por hora
            decimal montoPorHora = dto.MontoTotal / dto.DuracionHoras;
            const decimal MONTO_MINIMO_POR_HORA = 1500;
            if (montoPorHora < MONTO_MINIMO_POR_HORA)
            {
                throw new InvalidOperationException($"El monto por hora no puede ser menor a ${MONTO_MINIMO_POR_HORA}");
            }

            // Validar terapeutas
            if (dto.Terapeutas == null || !dto.Terapeutas.Any())
            {
                throw new InvalidOperationException("Debe asignar al menos una chica al servicio");
            }

            foreach (var terapeuta in dto.Terapeutas)
            {
                // Validar monto de terapeuta
                if (terapeuta.MontoTerapeuta < 1000)
                {
                    throw new InvalidOperationException("El monto de la chica no puede ser menor a $1,000");
                }

                // Validar monto mínimo por hora para terapeuta
                decimal montoTerapeutaPorHora = terapeuta.MontoTerapeuta / dto.DuracionHoras;
                const decimal MONTO_MINIMO_TERAPEUTA_HORA = 1000;
                if (montoTerapeutaPorHora < MONTO_MINIMO_TERAPEUTA_HORA)
                {
                    throw new InvalidOperationException($"El monto para la chica no puede ser menor a ${MONTO_MINIMO_TERAPEUTA_HORA} por ${dto.DuracionHoras} horas");
                }

                // Validar que el monto de la terapeuta no exceda el monto total
                if (terapeuta.MontoTerapeuta > dto.MontoTotal)
                {
                    throw new InvalidOperationException("El monto de la chica no puede ser mayor al monto total del servicio");
                }
            }

            // Validaciones específicas para servicio a domicilio
            if (dto.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO)
            {
                if (string.IsNullOrEmpty(dto.Direccion))
                {
                    throw new InvalidOperationException("La dirección es requerida para servicios a domicilio");
                }

                if (!dto.GastosTransporte.HasValue)
                {
                    throw new InvalidOperationException("Debe especificar los gastos de transporte (incluso si son 0) para servicios a domicilio");
                }

                if (dto.GastosTransporte == 0 && string.IsNullOrEmpty(dto.NotasTransporte))
                {
                    throw new InvalidOperationException("Si los gastos de transporte son 0, debe especificar el motivo en las notas de transporte (ej: 'Cliente recoge a la terapeuta', 'Cliente paga transporte directamente')");
                }
            }
            else // Si es CONSULTORIO
            {
                if (dto.GastosTransporte.HasValue)
                {
                    throw new InvalidOperationException("No se permiten gastos de transporte para servicios en consultorio");
                }
            }

            // Validar fecha del servicio
            if (dto.FechaServicio < DateTime.Now)
            {
                throw new InvalidOperationException("La fecha del servicio no puede ser en el pasado");
            }
        }

        public async Task<ServicioTerapeutaDto> ActualizarEstadoComprobanteAsync(int servicioTerapeutaId, int comprobanteId, UpdateComprobanteEstadoDto dto)
        {
            if (!PagosEnum.EstadosComprobante.Contains(dto.Estado))
                throw new InvalidOperationException("Estado no válido");

            if (dto.Estado == PagosEnum.EstadoComprobante.RECHAZADO && string.IsNullOrEmpty(dto.MotivoRechazo))
                throw new InvalidOperationException("Debe proporcionar un motivo de rechazo");

            var comprobante = await _context.ComprobantesPago
                .Include(cp => cp.ServicioTerapeuta)
                .FirstOrDefaultAsync(cp => cp.Id == comprobanteId &&
                                         cp.ServicioTerapeutaId == servicioTerapeutaId);

            if (comprobante == null)
                throw new InvalidOperationException("Comprobante no encontrado");

            var estadoAnterior = comprobante.Estado;
            comprobante.Estado = dto.Estado;
            comprobante.MotivoRechazo = dto.MotivoRechazo;
            comprobante.NotasComprobante = dto.NotasComprobante;

            // Registrar cambio de estado del comprobante
            await _bitacoraService.RegistrarCambioEstadoAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TablaMonitoreo.COMPROBANTES_PAGO,
                comprobante.Id.ToString(),
                estadoAnterior,
                dto.Estado,
                dto.MotivoRechazo ?? dto.NotasComprobante
            );

            // Si es rechazo, registrar motivo detallado
            if (dto.Estado == PagosEnum.EstadoComprobante.RECHAZADO)
            {
                await _bitacoraService.RegistrarAccionAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TipoAccion.RECHAZO_PAGO,
                    BitacoraEnum.TablaMonitoreo.COMPROBANTES_PAGO,
                    comprobante.Id.ToString(),
                    JsonSerializer.Serialize(new { EstadoAnterior = estadoAnterior }),
                    JsonSerializer.Serialize(new
                    {
                        MotivoRechazo = dto.MotivoRechazo,
                        Notas = dto.NotasComprobante
                    })
                );
            }

            // Actualizar estado del servicio
            var estadoServicioAnterior = comprobante.ServicioTerapeuta.Estado;
            await ActualizarEstadoServicioAsync(comprobante.ServicioTerapeuta);

            // Si cambió el estado del servicio, registrarlo
            if (comprobante.ServicioTerapeuta.Estado != estadoServicioAnterior)
            {
                await _bitacoraService.RegistrarCambioEstadoAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                    servicioTerapeutaId.ToString(),
                    estadoServicioAnterior,
                    comprobante.ServicioTerapeuta.Estado,
                    $"Actualización automática por cambio en comprobante {comprobanteId}"
                );
            }

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByIdAsync(servicioTerapeutaId);
        }

        private async Task ActualizarEstadoServicioAsync(ServiciosTerapeutasModel servicioTerapeuta)
        {
            var comprobantes = await _context.ComprobantesPago
                .Where(cp => cp.ServicioTerapeutaId == servicioTerapeuta.Id)
                .ToListAsync();

            // Verificar estados de comprobantes
            var tienePagoClienteConfirmado = comprobantes
                .Any(cp => cp.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE
                        && cp.Estado == PagosEnum.EstadoComprobante.PAGADO);

            var todosRechazados = comprobantes.All(cp => cp.Estado == PagosEnum.EstadoComprobante.RECHAZADO);
            var tienePorConfirmar = comprobantes
                .Any(cp => cp.Estado == PagosEnum.EstadoComprobante.POR_CONFIRMAR);

            // Actualizar estado del ServiciosTerapeutas
            if (todosRechazados)
            {
                servicioTerapeuta.Estado = EstadosEnum.Servicio.PENDIENTE;
            }
            else if (tienePagoClienteConfirmado)
            {
                servicioTerapeuta.Estado = EstadosEnum.Servicio.FINALIZADO;
            }
            else if (tienePorConfirmar)
            {
                servicioTerapeuta.Estado = EstadosEnum.Servicio.EN_PROCESO;
            }

            // Actualizar el estado del servicio principal
            var servicio = await _context.Servicios.FindAsync(servicioTerapeuta.ServicioId);
            if (servicio != null)
            {
                servicio.Estado = servicioTerapeuta.Estado;
                _context.Servicios.Update(servicio);
            }

            _context.ServiciosTerapeutas.Update(servicioTerapeuta);
            await _context.SaveChangesAsync();
        }

        private async Task ValidarComprobante(int servicioTerapeutaId, CreateComprobantePagoDto dto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Servicio)
                .Include(st => st.ServiciosExtra) // Añadir servicios extra
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            // Validar que el tipo de comprobante sea válido
            if (!PagosEnum.TiposComprobante.Contains(dto.TipoComprobante))
                throw new InvalidOperationException("Tipo de comprobante no válido");

            // Validar que el origen de pago sea válido
            if (!PagosEnum.OrigenesComprobante.Contains(dto.OrigenPago))
                throw new InvalidOperationException("Origen de pago no válido");

            // Validar monto no negativo
            if (dto.Monto <= 0)
                throw new InvalidOperationException("El monto debe ser mayor que cero");

            // Validar número de operación único si se proporciona
            if (!string.IsNullOrEmpty(dto.NumeroOperacion))
            {
                var existeNumeroOperacion = await _context.ComprobantesPago
                    .AnyAsync(cp => cp.NumeroOperacion == dto.NumeroOperacion);
                if (existeNumeroOperacion)
                    throw new InvalidOperationException("El número de operación ya existe en el sistema");
            }

            // Validar URL del comprobante según el tipo
            if (dto.TipoComprobante != PagosEnum.TipoComprobante.EFECTIVO &&
                string.IsNullOrEmpty(dto.UrlComprobante))
            {
                throw new InvalidOperationException("La URL del comprobante es requerida para pagos que no son en efectivo");
            }

            // Calcular monto total permitido incluyendo servicios extra
            var montoTotalServiciosExtra = servicioTerapeuta.ServiciosExtra?.Sum(se => se.Monto) ?? 0;
            var montoTotalPermitido = servicioTerapeuta.Servicio.MontoTotal + montoTotalServiciosExtra;

            // Validar que no exceda el monto total permitido
            var totalExistente = servicioTerapeuta.ComprobantesPago
                .Where(cp => cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .Sum(cp => cp.Monto);

            if (dto.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE)
            {
                if (totalExistente + dto.Monto > montoTotalPermitido)
                    throw new InvalidOperationException($"La suma de los comprobantes ({totalExistente + dto.Monto:C}) excede el monto total permitido ({montoTotalPermitido:C})");
            }
            else // Si es pago de comisión
            {
                if (totalExistente + dto.Monto > servicioTerapeuta.MontoTerapeuta)
                    throw new InvalidOperationException("La suma de los comprobantes excede el monto de la terapeuta");
            }

            // Validar URL del comprobante según el tipo de pago
            if (dto.TipoComprobante != PagosEnum.TipoComprobante.EFECTIVO &&
                string.IsNullOrEmpty(dto.UrlComprobante))
            {
                throw new InvalidOperationException("La URL del comprobante es requerida para pagos que no son en efectivo");
            }
        }

        public async Task<ServicioTerapeutaDto> AgregarComprobantesMultiplesAsync(
            int servicioTerapeutaId,
            CreateComprobantesMultiplesDto dto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Servicio)
                .Include(st => st.ServiciosExtra) // Añadir servicios extra
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            // Calcular monto total permitido incluyendo servicios extra
            var montoTotalServiciosExtra = servicioTerapeuta.ServiciosExtra?.Sum(se => se.Monto) ?? 0;
            var montoTotalPermitido = servicioTerapeuta.Servicio.MontoTotal + montoTotalServiciosExtra;

            // Validar monto total de todos los comprobantes
            var montoTotalNuevosComprobantes = dto.Comprobantes.Sum(c => c.Monto);
            var montoTotalExistente = servicioTerapeuta.ComprobantesPago
                .Where(cp => cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .Sum(cp => cp.Monto);

            if (montoTotalExistente + montoTotalNuevosComprobantes > montoTotalPermitido)
                throw new InvalidOperationException($"La suma de los comprobantes ({montoTotalExistente + montoTotalNuevosComprobantes:C}) excede el monto total permitido ({montoTotalPermitido:C})");

            // Procesar todos los comprobantes
            foreach (var comprobante in dto.Comprobantes)
            {
                // Validar comprobante individual
                await ValidarComprobante(servicioTerapeutaId, comprobante);

                var nuevoComprobante = new ComprobantePagoModel
                {
                    ServicioTerapeutaId = servicioTerapeutaId,
                    TipoComprobante = comprobante.TipoComprobante,
                    OrigenPago = comprobante.OrigenPago,
                    NumeroOperacion = comprobante.NumeroOperacion,
                    UrlComprobante = comprobante.UrlComprobante,
                    NotasComprobante = comprobante.NotasComprobante,
                    Estado = PagosEnum.EstadoComprobante.PENDIENTE,
                    FechaRegistro = DateTime.Now,
                    IdUsuarioRegistro = _currentUserService.GetUserId(),
                    Monto = comprobante.Monto
                };

                servicioTerapeuta.ComprobantesPago.Add(nuevoComprobante);
            }

            // Actualizar estado del ServicioTerapeuta
            if (servicioTerapeuta.Estado == EstadosEnum.Servicio.FINALIZADO)
            {
                servicioTerapeuta.Estado = EstadosEnum.Servicio.POR_CONFIRMAR;

                // Actualizar también el estado del servicio principal
                servicioTerapeuta.Servicio.Estado = EstadosEnum.Servicio.POR_CONFIRMAR;
                _context.Servicios.Update(servicioTerapeuta.Servicio);
                await _bitacoraService.RegistrarCambioEstadoAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TablaMonitoreo.SERVICIOS,
                    servicioTerapeuta.ServicioId.ToString(),
                    EstadosEnum.Servicio.FINALIZADO,
                    EstadosEnum.Servicio.POR_CONFIRMAR,
                    "Registro de comprobantes de pago"
                );
            }

            // Actualizar el estado de la comisión relacionada
            var comision = await _context.Comisiones
                .FirstOrDefaultAsync(c => c.ServicioId == servicioTerapeuta.ServicioId);

            if (comision != null)
            {
                comision.Estado = EstadosEnum.Comision.POR_CONFIRMAR;
            }

            await _context.SaveChangesAsync();

            // Solo enviar una alerta si es necesario
            await AlertarMontoInusualSiNecesario(servicioTerapeuta);

            return await GetServicioTerapeutaByIdAsync(servicioTerapeutaId);
        }


        private async Task<ResultadoConciliacionDto> ValidarMontosAsync(ServiciosTerapeutasModel servicioTerapeuta)
        {
            var resultado = new ResultadoConciliacionDto
            {
                Observaciones = new List<string>(),
                MontoTotal = servicioTerapeuta.Servicio.MontoTotal,
                MontoComprobantes = 0
            };

            // Obtener total de servicios extra
            var serviciosExtra = servicioTerapeuta.ServiciosExtra?.ToList() ?? new List<ServicioExtraModel>();
            var montoServiciosExtra = serviciosExtra.Sum(se => se.Monto);

            // Obtener total de comprobantes válidos
            var comprobantesValidos = servicioTerapeuta.ComprobantesPago
                .Where(cp => cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .ToList();

            resultado.MontoComprobantes = comprobantesValidos.Sum(cp => cp.Monto);

            // Calcular el monto total esperado (servicio base + servicios extra)
            var montoTotalEsperado = resultado.MontoTotal + montoServiciosExtra;
            resultado.Diferencia = resultado.MontoComprobantes - montoTotalEsperado;

            // Validar montos
            if (resultado.MontoComprobantes > montoTotalEsperado)
            {
                resultado.Observaciones.Add($"Los comprobantes exceden el monto total por {resultado.Diferencia:C}");
                resultado.RequiereRevision = true;
            }
            else if (resultado.MontoComprobantes < montoTotalEsperado)
            {
                resultado.Observaciones.Add($"Faltan comprobantes por {Math.Abs(resultado.Diferencia.Value):C}");
                resultado.RequiereRevision = true;
            }

            // Agregar información de servicios extra
            if (serviciosExtra.Any())
            {
                var detalleServiciosExtra = string.Join(", ",
                    serviciosExtra.Select(se => $"{se.ServicioExtraCatalogo.Nombre}: {se.Monto:C}"));
                resultado.Observaciones.Add($"Servicios Extra (100% para terapeuta) - Total: {montoServiciosExtra:C} ({detalleServiciosExtra})");
            }

            // Validar coherencia de tipos de comprobante
            var tieneEfectivo = comprobantesValidos.Any(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.EFECTIVO);
            var tieneTransferencia = comprobantesValidos.Any(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.TRANSFERENCIA);

            if (tieneEfectivo && tieneTransferencia)
            {
                var totalEfectivo = comprobantesValidos
                    .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.EFECTIVO)
                    .Sum(cp => cp.Monto);

                var totalTransferencia = comprobantesValidos
                    .Where(cp => cp.TipoComprobante == PagosEnum.TipoComprobante.TRANSFERENCIA)
                    .Sum(cp => cp.Monto);

                resultado.Observaciones.Add($"Pago mixto - Efectivo: {totalEfectivo:C}, Transferencia: {totalTransferencia:C}");
            }

            // Calcular montos finales para la terapeuta
            var montoBaseTerapeuta = servicioTerapeuta.MontoTerapeuta ?? 0;
            var montoTotalTerapeuta = montoBaseTerapeuta + montoServiciosExtra;
            resultado.Observaciones.Add($"Monto total para terapeuta: {montoTotalTerapeuta:C} (Base: {montoBaseTerapeuta:C} + Extras: {montoServiciosExtra:C})");

            resultado.Exitoso = !resultado.RequiereRevision;
            return resultado;
        }

        public async Task<ConciliacionServicioDto> GetConciliacionServicioAsync(int servicioTerapeutaId)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Terapeuta)
                .Include(st => st.ServiciosExtra)  // Incluir servicios extra
                    .ThenInclude(se => se.ServicioExtraCatalogo)  // Incluir el catálogo para obtener nombres
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            var resultadoConciliacion = await ValidarMontosAsync(servicioTerapeuta);

            // Mapear servicios extra
            var serviciosExtraDto = servicioTerapeuta.ServiciosExtra?
                .Select(se => new ServicioExtraDetalleDto
                {
                    Id = se.Id,
                    ServicioExtraCatalogoId = se.ServicioExtraCatalogoId,
                    NombreServicio = se.ServicioExtraCatalogo.Nombre,
                    Monto = se.Monto,
                    FechaRegistro = se.FechaRegistro,
                    Notas = se.Notas
                }).ToList() ?? new List<ServicioExtraDetalleDto>();

            return new ConciliacionServicioDto
            {
                ServicioId = servicioTerapeuta.ServicioId,
                TerapeutaId = servicioTerapeuta.TerapeutaId,
                FechaServicio = servicioTerapeuta.Servicio.FechaServicio,
                TipoUbicacion = servicioTerapeuta.Servicio.TipoUbicacion,
                MontoTotal = servicioTerapeuta.Servicio.MontoTotal,
                MontoTerapeuta = servicioTerapeuta.MontoTerapeuta ?? 0,
                GastosTransporte = servicioTerapeuta.GastosTransporte,
                Estado = servicioTerapeuta.Estado,
                ResultadoConciliacion = resultadoConciliacion.Exitoso ? "EXITOSO" : "REQUIERE_REVISION",
                Discrepancias = resultadoConciliacion.Observaciones,
                MontoServiciosExtra = servicioTerapeuta.ServiciosExtra?.Sum(se => se.Monto) ?? 0,
                ServiciosExtra = serviciosExtraDto,
                Comprobantes = servicioTerapeuta.ComprobantesPago
                    .Select(cp => new ComprobanteConciliacionDto
                    {
                        Id = cp.Id,
                        TipoComprobante = cp.TipoComprobante,
                        OrigenPago = cp.OrigenPago,
                        Monto = cp.Monto,
                        Estado = cp.Estado,
                        FechaRegistro = cp.FechaRegistro,
                        NumeroOperacion = cp.NumeroOperacion
                    }).ToList()
            };
        }

        public async Task<ResultadoConciliacionDto> RealizarConciliacionAsync(int servicioTerapeutaId)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Terapeuta)
                .Include(st => st.ServiciosExtra) // Incluir servicios extra
                    .ThenInclude(se => se.ServicioExtraCatalogo)
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            var resultado = await ValidarMontosAsync(servicioTerapeuta);

            // Calcular montos incluyendo servicios extra
            var montoServiciosExtra = servicioTerapeuta.ServiciosExtra?.Sum(se => se.Monto) ?? 0;
            var montoTotalEsperado = servicioTerapeuta.Servicio.MontoTotal + montoServiciosExtra;
            var montoComprobantes = servicioTerapeuta.ComprobantesPago
                .Where(cp => cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .Sum(cp => cp.Monto);

            resultado.MontoTotal = montoTotalEsperado;
            resultado.MontoComprobantes = montoComprobantes;
            resultado.Diferencia = montoComprobantes - montoTotalEsperado;
            resultado.RequiereRevision = Math.Abs(resultado.Diferencia ?? 0) > 0;
            resultado.Exitoso = !resultado.RequiereRevision;

            // Registrar el proceso de conciliación en la bitácora
            await _bitacoraService.RegistrarAccionAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TipoAccion.CONCILIACION,
                BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                servicioTerapeutaId.ToString(),
                null,
                JsonSerializer.Serialize(new
                {
                    MontoTotal = montoTotalEsperado,
                    MontoComprobantes = montoComprobantes,
                    MontoServiciosExtra = montoServiciosExtra,
                    Diferencia = resultado.Diferencia,
                    ServiciosExtra = servicioTerapeuta.ServiciosExtra.Select(se => new
                    {
                        se.ServicioExtraCatalogo.Nombre,
                        se.Monto
                    }).ToList()
                })
            );

            if (resultado.Exitoso)
            {
                // Al generar la comisión, establecer el estado correcto
                var comision = await GenerarComisionAsync(servicioTerapeuta);
                comision.Estado = EstadosEnum.Comision.POR_CONFIRMAR;

                resultado.Observaciones.Add($"Comisión generada automáticamente: {comision.Id}");

                await _bitacoraService.RegistrarAccionAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TipoAccion.GENERACION_COMISION,
                    BitacoraEnum.TablaMonitoreo.COMISIONES,
                    comision.Id.ToString(),
                    null,
                    JsonSerializer.Serialize(new
                    {
                        MontoComision = comision.MontoComisionTotal,
                        FechaGeneracion = DateTime.Now,
                        MontoServiciosExtra = montoServiciosExtra
                    })
                );
            }

            return resultado;
        }

        public async Task<bool> ValidarConciliacionAsync(int servicioTerapeutaId)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            var resultado = await ValidarMontosAsync(servicioTerapeuta);

            // Registrar el resultado de la validación
            await _bitacoraService.RegistrarValidacionAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                servicioTerapeutaId.ToString(),
                resultado.Exitoso,
                string.Join(", ", resultado.Observaciones)
            );

            return resultado.Exitoso;
        }

        private async Task<ComisionesModel> GenerarComisionAsync(ServiciosTerapeutasModel servicioTerapeuta)
        {
            // Cálculo de montos
            // El monto total del servicio es 1800
            var montoTotal = servicioTerapeuta.Servicio.MontoTotal;

            // El monto para la terapeuta es 1100
            var montoTerapeuta = servicioTerapeuta.MontoTerapeuta ?? 0;

            // La comisión total es la diferencia: 1800 - 1100 = 700
            var montoComisionTotal = montoTotal - montoTerapeuta;

            var comision = new ComisionesModel
            {
                ServicioId = servicioTerapeuta.ServicioId,
                TerapeutaId = servicioTerapeuta.TerapeutaId,
                PresentadorId = servicioTerapeuta.Servicio.PresentadorId,
                MontoTotal = montoTotal,
                MontoTerapeuta = servicioTerapeuta.MontoTerapeuta ?? 0,
                MontoComisionTotal = montoComisionTotal,
                MontoComisionEmpresa = montoComisionTotal * 0.70m, // 70% para la empresa
                MontoComisionPresentador = montoComisionTotal * 0.30m, // 30% para el presentador
                PorcentajeAplicadoEmpresa = 70,
                PorcentajeAplicadoPresentador = 30,
                FechaCalculo = DateTime.Now,
                Estado = EstadosEnum.Comision.NO_PAGADO
            };

            _context.Comisiones.Add(comision);
            await _context.SaveChangesAsync();

            return comision;
        }

        private async Task ValidarServiciosSimultaneosAsync(int terapeutaId, DateTime horaInicio, DateTime horaFin)
        {
            var serviciosActivos = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Where(st => st.TerapeutaId == terapeutaId &&
                            st.Estado == EstadosEnum.Servicio.EN_PROCESO &&
                            st.HoraInicio.HasValue)
                .ToListAsync();

            var serviciosSimultaneos = serviciosActivos
                .Where(st => (horaInicio >= st.HoraInicio && horaInicio <= st.HoraFin) ||
                             (horaFin >= st.HoraInicio && horaFin <= st.HoraFin))
                .ToList();

            if (serviciosSimultaneos.Any())
            {
                var serviciosIds = serviciosSimultaneos.Select(s => s.ServicioId).ToArray();
                await _whatsAppService.AlertarServiciosSimultaneosAsync(terapeutaId, serviciosIds);

                // Registramos en bitácora
                await _bitacoraService.RegistrarValidacionAsync(
                    terapeutaId.ToString(),
                    BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                    string.Join(",", serviciosIds),
                    false,
                    $"Alerta: Se detectaron {serviciosIds.Length} servicios simultáneos"
                );
            }
        }

        private async Task ValidarFinalizacionServicio(ServiciosTerapeutasModel servicioTerapeuta, DateTime horaFin, double latitud, double longitud)
        {
            // 1. Validación de duración
            var duracionServicio = horaFin - servicioTerapeuta.HoraInicio.Value;
            var duracionEsperada = TimeSpan.FromHours(servicioTerapeuta.Servicio.DuracionHoras);
            var margenTiempo = TimeSpan.FromMinutes(30);

            // Si se excede del tiempo programado + margen
            if (duracionServicio > duracionEsperada.Add(margenTiempo))
            {
                var exceso = duracionServicio - duracionEsperada;
                await _whatsAppService.AlertarServicioExtendidoAsync(
                    servicioTerapeuta.ServicioId,
                    $"⏱ Servicio extendido\n" +
                    $"Tiempo programado: {duracionEsperada.TotalHours:F1} horas\n" +
                    $"Tiempo real: {duracionServicio.TotalHours:F1} horas\n" +
                    $"Exceso: {exceso.TotalMinutes:F0} minutos");

                await _bitacoraService.RegistrarValidacionAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                    servicioTerapeuta.Id.ToString(),
                    false,
                    $"Servicio extendido por {exceso.TotalMinutes:F0} minutos"
                );
            }
            // Si termina antes del tiempo programado - margen
            else if (duracionServicio < duracionEsperada.Subtract(margenTiempo))
            {
                var faltante = duracionEsperada - duracionServicio;
                await _whatsAppService.AlertarServicioExtendidoAsync(
                    servicioTerapeuta.ServicioId,
                    $"⏱ Servicio corto\n" +
                    $"Tiempo programado: {duracionEsperada.TotalHours:F1} horas\n" +
                    $"Tiempo real: {duracionServicio.TotalHours:F1} horas\n" +
                    $"Faltaron: {faltante.TotalMinutes:F0} minutos");

                await _bitacoraService.RegistrarValidacionAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                    servicioTerapeuta.Id.ToString(),
                    false,
                    $"Servicio corto por {faltante.TotalMinutes:F0} minutos"
                );
            }

            // 2. Validación de ubicación para servicios a domicilio
            if (servicioTerapeuta.Servicio.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO)
            {
                var ubicacionInicial = servicioTerapeuta.UbicacionInicio;
                if (ubicacionInicial != null)
                {
                    var distancia = _locationService.CalculateDistance(
                        ubicacionInicial.Coordinate.Y,
                        ubicacionInicial.Coordinate.X,
                        latitud,
                        longitud
                    );

                    if (distancia < 0.1) // 100 metros
                    {
                        await _whatsAppService.AlertarAnomaliaUbicacionAsync(
                            servicioTerapeuta.ServicioId,
                            "📍 Ubicación Sospechosa",
                            $"🚨 La ubicación no cambió. Servicio a domicilio finalizado en la misma ubicación que inició.");

                        await _bitacoraService.RegistrarValidacionAsync(
                            _currentUserService.GetUserId(),
                            BitacoraEnum.TablaMonitoreo.SERVICIOS_TERAPEUTAS,
                            servicioTerapeuta.Id.ToString(),
                            false,
                            "Servicio a domicilio finalizado en la misma ubicación inicial."
                        );
                    }
                }
            }
        }

        private async Task ValidarComprobantePago(ServiciosTerapeutasModel servicioTerapeuta, CreateComprobantePagoDto dto)
        {
            const decimal MONTO_MINIMO_ALERTA_POR_HORA = 1500m;

            // Validación diferenciada según origen del pago
            if (dto.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE)
            {
                // Para pago de cliente, validamos el monto por hora del servicio completo
                decimal montoPorHora = dto.Monto / servicioTerapeuta.Servicio.DuracionHoras;
                if (montoPorHora < MONTO_MINIMO_ALERTA_POR_HORA)
                {
                    await _whatsAppService.AlertarMontoInusualAsync(
                        servicioTerapeuta.ServicioId,
                        montoPorHora
                    );
                }
            }
            else if (dto.OrigenPago == PagosEnum.OrigenPago.COMISION_TERAPEUTA)
            {
                // Para pago a terapeuta, validamos que no sea menor al mínimo por hora para terapeuta
                decimal montoPorHora = dto.Monto / servicioTerapeuta.Servicio.DuracionHoras;
                if (montoPorHora < 1000) // Monto mínimo por hora para terapeuta
                {
                    await _whatsAppService.AlertarMontoInusualAsync(
                        servicioTerapeuta.ServicioId,
                        montoPorHora
                    );
                }
            }

            // 2. Validar coherencia de montos
            var montosActuales = await _context.ComprobantesPago
                .Where(cp => cp.ServicioTerapeutaId == servicioTerapeuta.Id)
                .Where(cp => cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .SumAsync(cp => cp.Monto);

            var montoTotalConNuevoComprobante = montosActuales + dto.Monto;

            if (montoTotalConNuevoComprobante > servicioTerapeuta.MontoTerapeuta)
            {
                await _whatsAppService.AlertarMontoInusualAsync(
                    servicioTerapeuta.ServicioId,
                    montoTotalConNuevoComprobante  // Aquí también pasamos solo el monto
                );

                await _bitacoraService.RegistrarValidacionAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TablaMonitoreo.COMPROBANTES_PAGO,
                    servicioTerapeuta.Id.ToString(),
                    false,
                    $"Suma de comprobantes (${montoTotalConNuevoComprobante:N2}) excede el monto total (${servicioTerapeuta.MontoTerapeuta:N2})"
                );
            }

            // 3. Validar imagen adjunta
            if (string.IsNullOrEmpty(dto.UrlComprobante))
            {
                await _whatsAppService.AlertarErrorConciliacionAsync(
                    servicioTerapeuta.ServicioId,
                    "Comprobante registrado sin imagen adjunta"
                );

                await _bitacoraService.RegistrarValidacionAsync(
                    _currentUserService.GetUserId(),
                    BitacoraEnum.TablaMonitoreo.COMPROBANTES_PAGO,
                    servicioTerapeuta.Id.ToString(),
                    false,
                    "Comprobante registrado sin imagen adjunta"
                );
            }
        }

        public async Task NotificarCancelacionesExcesivasAsync()
        {
            var cancelaciones = await _context.CancelacionesPresentador.ToListAsync();

            foreach (var cancelacion in cancelaciones)
            {
                var mensaje = $"⚠️ ALERTA: Cancelaciones Excesivas\n" +
                             $"Presentador: {cancelacion.NombrePresentador}\n" +
                             $"Cancelaciones última semana: {cancelacion.CantidadCancelaciones}\n" +
                             $"Período: {cancelacion.SemanaInicio:dd/MM/yyyy} a {cancelacion.SemanaFin:dd/MM/yyyy}";

                await _whatsAppService.AlertarCancelacionesExcesivasAsync(cancelacion.PresentadorId, mensaje);
            }
        }

        public async Task<ServicioExtraDetalleDto> UpdateServicioExtraAsync(int servicioTerapeutaId, int servicioExtraId, UpdateServicioExtraDto dto)
        {
            var servicioExtra = await _context.ServiciosExtra
                .Include(se => se.ServicioTerapeuta)
                .Include(se => se.ServicioExtraCatalogo)
                .FirstOrDefaultAsync(se =>
                    se.Id == servicioExtraId &&
                    se.ServicioTerapeutaId == servicioTerapeutaId);

            if (servicioExtra == null)
                throw new InvalidOperationException("Servicio extra no encontrado");

            if (servicioExtra.ServicioTerapeuta.Estado == "PAGADO")
                throw new InvalidOperationException("No se puede modificar un servicio extra cuando el servicio está pagado");

            if (dto.Monto <= 0)
                throw new InvalidOperationException("El monto debe ser mayor a 0");

            servicioExtra.Monto = dto.Monto;
            servicioExtra.Notas = dto.Notas;

            await _context.SaveChangesAsync();

            return new ServicioExtraDetalleDto
            {
                Id = servicioExtra.Id,
                ServicioExtraCatalogoId = servicioExtra.ServicioExtraCatalogoId,
                NombreServicio = servicioExtra.ServicioExtraCatalogo.Nombre,
                Monto = servicioExtra.Monto,
                FechaRegistro = servicioExtra.FechaRegistro,
                Notas = servicioExtra.Notas
            };
        }

        public async Task<bool> DeleteServicioExtraAsync(int servicioTerapeutaId, int servicioExtraId)
        {
            var servicioExtra = await _context.ServiciosExtra
                .Include(se => se.ServicioTerapeuta)
                .FirstOrDefaultAsync(se =>
                    se.Id == servicioExtraId &&
                    se.ServicioTerapeutaId == servicioTerapeutaId);

            if (servicioExtra == null)
                throw new InvalidOperationException("Servicio extra no encontrado");

            if (servicioExtra.ServicioTerapeuta.Estado == "PAGADO")
                throw new InvalidOperationException("No se puede eliminar un servicio extra cuando el servicio está pagado");

            _context.ServiciosExtra.Remove(servicioExtra);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task AlertarMontoInusualSiNecesario(ServiciosTerapeutasModel servicioTerapeuta)
        {
            var montoTotalComprobantes = servicioTerapeuta.ComprobantesPago
                .Where(cp => cp.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE &&
                             cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .Sum(cp => cp.Monto);

            if (montoTotalComprobantes > 0)
            {
                var montoPorHora = montoTotalComprobantes / servicioTerapeuta.Servicio.DuracionHoras;
                await _whatsAppService.AlertarMontoInusualAsync(servicioTerapeuta.ServicioId, montoPorHora);
            }
        }

        public async Task EliminarComprobantePagoAsync(int servicioTerapeutaId, int comprobanteId)
        {
            var comprobante = await _context.ComprobantesPago
                .FirstOrDefaultAsync(cp => cp.Id == comprobanteId &&
                                         cp.ServicioTerapeutaId == servicioTerapeutaId);

            if (comprobante == null)
                throw new InvalidOperationException("Comprobante no encontrado");

            _context.ComprobantesPago.Remove(comprobante);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ComprobantePagoDto>> GetComprobantesPagoByServicioAsync(int servicioTerapeutaId)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                    .ThenInclude(cp => cp.UsuarioRegistro)
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            return servicioTerapeuta.ComprobantesPago
                .Select(cp => new ComprobantePagoDto
                {
                    Id = cp.Id,
                    ServicioTerapeutaId = cp.ServicioTerapeutaId,
                    TipoComprobante = cp.TipoComprobante,
                    OrigenPago = cp.OrigenPago,
                    NumeroOperacion = cp.NumeroOperacion,
                    UrlComprobante = cp.UrlComprobante,
                    FechaRegistro = cp.FechaRegistro,
                    Estado = cp.Estado,
                    NotasComprobante = cp.NotasComprobante,
                    MotivoRechazo = cp.MotivoRechazo,
                    NombreUsuarioRegistro = cp.UsuarioRegistro?.UserName ?? "Sistema",
                    Monto = cp.Monto
                })
                .OrderByDescending(cp => cp.FechaRegistro)
                .ToList();
        }
    }
}
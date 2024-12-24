using System;
using System.Collections.Generic;
using System.Linq;
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

        public ServicioService(
            ApplicationDbContext context,
            ILocationService locationService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _locationService = locationService;
            _currentUserService = currentUserService;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task<List<ServicioDto>> GetAllAsync()
        {
            var servicios = await _context.Servicios
                .Include(s => s.Presentador)
                .Include(s => s.ServiciosTerapeutas)
                    .ThenInclude(st => st.Terapeuta)
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
                TipoUbicacion = dto.TipoUbicacion, // Cambiar TipoServicio por TipoUbicacion
                Direccion = dto.Direccion,
                MontoTotal = dto.MontoTotal,
                GastosTransporte = dto.GastosTransporte,
                NotasTransporte = dto.NotasTransporte,
                Estado = EstadosEnum.Servicio.PENDIENTE,
                Notas = dto.Notas
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

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
                Estado = servicio.Estado,
                FechaCancelacion = servicio.FechaCancelacion,
                MotivoCancelacion = servicio.MotivoCancelacion,
                NotasCancelacion = servicio.NotasCancelacion,
                Notas = servicio.Notas,
                NombrePresentador = $"{servicio.Presentador?.Nombre} {servicio.Presentador?.Apellido}".Trim(),
                Terapeutas = servicio.ServiciosTerapeutas?
                    .Select(MapToServicioTerapeutaDto)
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
                .FirstOrDefaultAsync(st => st.LinkConfirmacion == confirmacionDto.LinkConfirmacion);

            if (servicioTerapeuta == null) return null;

            servicioTerapeuta.HoraInicio = DateTime.Now;
            servicioTerapeuta.Estado = EstadosEnum.Servicio.EN_PROCESO;
            servicioTerapeuta.UbicacionInicio = _geometryFactory.CreatePoint(
                new Coordinate(confirmacionDto.Longitud, confirmacionDto.Latitud));

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByLinkConfirmacionAsync(confirmacionDto.LinkConfirmacion);
        }

        public async Task<ServicioTerapeutaDto> FinalizarServicioAsync(FinalizacionServicioDto finalizacionDto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .Include(st => st.ComprobantesPago)
                .FirstOrDefaultAsync(st => st.LinkFinalizacion == finalizacionDto.LinkFinalizacion);

            if (servicioTerapeuta == null) return null;

            servicioTerapeuta.HoraFin = DateTime.Now;
            servicioTerapeuta.Estado = EstadosEnum.Servicio.FINALIZADO;
            servicioTerapeuta.UbicacionFin = _geometryFactory.CreatePoint(
                new Coordinate(finalizacionDto.Longitud, finalizacionDto.Latitud));

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
                .FirstOrDefaultAsync(st => st.ServicioId == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            if (!string.IsNullOrEmpty(dto.NumeroOperacion))
            {
                var esUnico = await ValidarNumeroOperacionUnico(dto.NumeroOperacion);
                if (!esUnico)
                    throw new InvalidOperationException("El número de operación ya existe en el sistema");
            }

            var comprobante = new ComprobantePagoModel
            {
                ServicioTerapeutaId = servicioTerapeutaId,
                TipoComprobante = dto.TipoComprobante,
                NumeroOperacion = dto.NumeroOperacion,
                UrlComprobante = dto.UrlComprobante,
                NotasComprobante = dto.NotasComprobante,
                Estado = PagosEnum.EstadoComprobante.PENDIENTE,
                FechaRegistro = DateTime.Now,
                IdUsuarioRegistro = _currentUserService.GetUserId()
            };

            servicioTerapeuta.ComprobantesPago.Add(comprobante);

            // Actualizar estado del servicio si es el primer comprobante
            if (servicioTerapeuta.Estado == EstadosEnum.Servicio.FINALIZADO)
                servicioTerapeuta.Estado = EstadosEnum.Comision.POR_CONFIRMAR;

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByIdAsync(servicioTerapeutaId);
        }

        public async Task<ServicioTerapeutaDto> ActualizarEstadoComprobanteAsync(
    int servicioTerapeutaId,
    int comprobanteId,
    string nuevoEstado)
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

            // Validaciones específicas para servicio a domicilio
            if (dto.TipoUbicacion == ServicioEnum.TipoUbicacion.DOMICILIO)
            {
                if (string.IsNullOrEmpty(dto.Direccion))
                {
                    throw new InvalidOperationException("La dirección es requerida para servicios a domicilio");
                }

                // Validar gastos de transporte para servicio a domicilio
                if (!dto.GastosTransporte.HasValue || dto.GastosTransporte < 0)
                {
                    throw new InvalidOperationException("Los gastos de transporte son requeridos y deben ser mayores a 0 para servicios a domicilio");
                }
            }
            else // Si es CONSULTORIO
            {
                // Asegurar que no se envíen gastos de transporte para servicios en consultorio
                if (dto.GastosTransporte.HasValue)
                {
                    throw new InvalidOperationException("No se permiten gastos de transporte para servicios en consultorio");
                }
            }

            // Validar que el monto total sea mayor que cero
            if (dto.MontoTotal <= 0)
            {
                throw new InvalidOperationException("El monto total debe ser mayor que cero");
            }
        }

        private async Task ValidateComprobante(int servicioTerapeutaId, CreateComprobantePagoDto dto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ComprobantesPago)
                .Include(st => st.Servicio)
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
            {
                throw new InvalidOperationException("Servicio no encontrado");
            }

            // Validar que el tipo de comprobante sea válido
            if (!PagosEnum.TiposComprobante.Contains(dto.TipoComprobante))
            {
                throw new InvalidOperationException("Tipo de comprobante no válido");
            }

            // Validar que el origen de pago sea válido
            if (!PagosEnum.OrigenesComprobante.Contains(dto.OrigenPago))
            {
                throw new InvalidOperationException("Origen de pago no válido");
            }

            // Validar que las comisiones solo se registren después del pago del cliente
            if (dto.OrigenPago == PagosEnum.OrigenPago.COMISION_TERAPEUTA)
            {
                var existePagoCliente = servicioTerapeuta.ComprobantesPago
                    .Any(cp => cp.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE
                           && cp.Estado == PagosEnum.EstadoComprobante.PAGADO);

                if (!existePagoCliente)
                {
                    throw new InvalidOperationException("No se puede registrar la comisión hasta que el pago del cliente esté confirmado");
                }
            }

            // Validar número de operación único si se proporciona
            if (!string.IsNullOrEmpty(dto.NumeroOperacion))
            {
                var existeNumeroOperacion = await _context.ComprobantesPago
                    .AnyAsync(cp => cp.NumeroOperacion == dto.NumeroOperacion);

                if (existeNumeroOperacion)
                {
                    throw new InvalidOperationException("El número de operación ya existe en el sistema");
                }
            }

            // Validar montos según el tipo de pago
            var totalComprobantes = servicioTerapeuta.ComprobantesPago
                .Where(cp => cp.OrigenPago == dto.OrigenPago
                          && cp.Estado != PagosEnum.EstadoComprobante.RECHAZADO)
                .Sum(cp => cp.Monto);

            if (dto.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE
                && (totalComprobantes + dto.Monto) > servicioTerapeuta.Servicio.MontoTotal)
            {
                throw new InvalidOperationException("La suma de los comprobantes excede el monto total del servicio");
            }

            // Validar que solo servicios a domicilio tengan gastos de transporte
            if (servicioTerapeuta.GastosTransporte.HasValue &&
                servicioTerapeuta.Servicio.TipoUbicacion != ServicioEnum.TipoUbicacion.DOMICILIO)
            {
                throw new InvalidOperationException("No se permiten gastos de transporte para servicios en consultorio");
            }
        }

        public async Task<ServicioTerapeutaDto> ActualizarEstadoComprobanteAsync(
            int servicioTerapeutaId,
            int comprobanteId,
            UpdateComprobanteEstadoDto dto)
        {
            if (!PagosEnum.EstadosComprobante.Contains(dto.Estado))
                throw new InvalidOperationException("Estado no válido");

            // Validar que si es rechazo, tenga motivo
            if (dto.Estado == PagosEnum.EstadoComprobante.RECHAZADO && string.IsNullOrEmpty(dto.MotivoRechazo))
                throw new InvalidOperationException("Debe proporcionar un motivo de rechazo");

            var comprobante = await _context.ComprobantesPago
                .Include(cp => cp.ServicioTerapeuta)  // Incluir ServicioTerapeuta
                .FirstOrDefaultAsync(cp => cp.Id == comprobanteId &&
                                         cp.ServicioTerapeutaId == servicioTerapeutaId);

            if (comprobante == null)
                throw new InvalidOperationException("Comprobante no encontrado");

            comprobante.Estado = dto.Estado;
            comprobante.MotivoRechazo = dto.MotivoRechazo;
            comprobante.NotasComprobante = dto.NotasComprobante;

            // Actualizar estado del servicio
            await ActualizarEstadoServicioAsync(comprobante.ServicioTerapeuta);

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByIdAsync(servicioTerapeutaId);
        }

        private async Task ActualizarEstadoServicioAsync(ServiciosTerapeutasModel servicioTerapeuta)
        {
            var comprobantes = await _context.ComprobantesPago
                .Where(cp => cp.ServicioTerapeutaId == servicioTerapeuta.Id)
                .ToListAsync();

            // Verificar si hay algún comprobante de pago de cliente confirmado
            var tienePagoClienteConfirmado = comprobantes
                .Any(cp => cp.OrigenPago == PagosEnum.OrigenPago.PAGO_CLIENTE
                        && cp.Estado == PagosEnum.EstadoComprobante.PAGADO);

            // Verificar si todos los comprobantes están rechazados
            var todosRechazados = comprobantes.All(cp => cp.Estado == PagosEnum.EstadoComprobante.RECHAZADO);

            // Verificar si hay comprobantes por confirmar
            var tienePorConfirmar = comprobantes
                .Any(cp => cp.Estado == PagosEnum.EstadoComprobante.POR_CONFIRMAR);

            // Actualizar el estado del servicio según las condiciones
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

            _context.ServiciosTerapeutas.Update(servicioTerapeuta);
        }
    }
}
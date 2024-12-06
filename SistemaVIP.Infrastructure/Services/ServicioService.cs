using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
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
        private readonly GeometryFactory _geometryFactory;

        public ServicioService(ApplicationDbContext context, ILocationService locationService)
        {
            _context = context;
            _locationService = locationService;
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

        public async Task<ServicioDto> CreateAsync(CreateServicioDto createDto)
        {
            // Validar presentador y terapeutas
            var isValid = await ValidateServicioTerapeutasAsync(createDto.Terapeutas, createDto.PresentadorId);

            var servicio = new ServiciosModel
            {
                PresentadorId = createDto.PresentadorId,
                FechaSolicitud = DateTime.Now,
                FechaServicio = createDto.FechaServicio,
                TipoServicio = createDto.TipoServicio,
                Direccion = createDto.Direccion,
                MontoTotal = createDto.MontoTotal,
                Estado = EstadosEnum.Servicio.PENDIENTE,
                Notas = createDto.Notas
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            foreach (var terapeutaDto in createDto.Terapeutas)
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
                TipoServicio = servicio.TipoServicio,
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
                ServicioId = st.ServicioId,
                TerapeutaId = st.TerapeutaId,
                NombreTerapeuta = $"{st.Terapeuta?.Nombre} {st.Terapeuta?.Apellido}".Trim(),
                FechaAsignacion = st.FechaAsignacion,
                HoraInicio = st.HoraInicio,
                HoraFin = st.HoraFin,
                Estado = st.Estado,
                MontoTerapeuta = st.MontoTerapeuta,
                LinkConfirmacion = st.LinkConfirmacion,
                LinkFinalizacion = st.LinkFinalizacion,
                ComprobantePagoTerapeuta = st.ComprobantePagoTerapeuta
            };
        }

        public async Task<ServicioTerapeutaDto> ConfirmarInicioServicioAsync(ConfirmacionServicioDto confirmacionDto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .FirstOrDefaultAsync(st => st.LinkConfirmacion == confirmacionDto.LinkConfirmacion);

            if (servicioTerapeuta == null) return null;

            servicioTerapeuta.HoraInicio = DateTime.Now;
            servicioTerapeuta.Estado = "EnProceso";
            servicioTerapeuta.UbicacionInicio = _geometryFactory.CreatePoint(
                new Coordinate(confirmacionDto.Longitud, confirmacionDto.Latitud));

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByLinkConfirmacionAsync(confirmacionDto.LinkConfirmacion);
        }

        public async Task<ServicioTerapeutaDto> FinalizarServicioAsync(FinalizacionServicioDto finalizacionDto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.Servicio)
                .FirstOrDefaultAsync(st => st.LinkFinalizacion == finalizacionDto.LinkFinalizacion);

            if (servicioTerapeuta == null) return null;

            servicioTerapeuta.HoraFin = DateTime.Now;
            servicioTerapeuta.Estado = "Finalizado";
            servicioTerapeuta.UbicacionFin = _geometryFactory.CreatePoint(
                new Coordinate(finalizacionDto.Longitud, finalizacionDto.Latitud));

            // Validar distancia si hay ubicación inicial
            if (servicioTerapeuta.UbicacionInicio != null)
            {
                var distanciaValida = await ValidateLocationDistanceAsync(
                    servicioTerapeuta.UbicacionInicio.Y, // Latitud
                    servicioTerapeuta.UbicacionInicio.X, // Longitud
                    finalizacionDto.Latitud,
                    finalizacionDto.Longitud);

                if (!distanciaValida)
                {
                    servicioTerapeuta.Notas = (servicioTerapeuta.Notas ?? "") +
                        "\nAlerta: Posible discrepancia en ubicación de finalización";
                }
            }

            await _context.SaveChangesAsync();
            return await GetServicioTerapeutaByLinkFinalizacionAsync(finalizacionDto.LinkFinalizacion);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class BlacklistService : IBlacklistService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBitacoraService _bitacoraService;

        public BlacklistService(
            ApplicationDbContext context,
            ICurrentUserService currentUserService,
            IBitacoraService bitacoraService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _bitacoraService = bitacoraService;
        }

        public async Task<List<BlacklistDto>> GetAllAsync()
        {
            var registros = await _context.Blacklist
                .Include(b => b.UsuarioRegistro)
                .OrderByDescending(b => b.FechaRegistro)
                .ToListAsync();

            return registros.Select(MapToDto).ToList();
        }

        public async Task<BlacklistDto> GetByIdAsync(int id)
        {
            var registro = await _context.Blacklist
                .Include(b => b.UsuarioRegistro)
                .FirstOrDefaultAsync(b => b.Id == id);

            return registro != null ? MapToDto(registro) : null;
        }

        public async Task<BlacklistDto> CreateAsync(CreateBlacklistDto dto)
        {
            // Validar datos del registro
            var validacionResult = await ValidarDatosRegistroAsync(dto);
            if (!validacionResult)
                throw new InvalidOperationException("Los datos proporcionados no son válidos");

            var registro = new BlacklistModel
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Motivo = dto.Motivo,
                FechaRegistro = DateTime.UtcNow,
                IdUsuarioRegistro = _currentUserService.GetUserId(),
                Estado = EstadosEnum.General.ACTIVO,
                Notas = dto.Notas
            };

            _context.Blacklist.Add(registro);
            await _context.SaveChangesAsync();

            // Registrar en bitácora
            await _bitacoraService.RegistrarAccionAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TipoAccion.CREACION,
                BitacoraEnum.TablaMonitoreo.BLACKLIST,
                registro.Id.ToString(),
                null,
                System.Text.Json.JsonSerializer.Serialize(new { dto.Motivo, dto.Telefono, dto.Email })
            );

            return await GetByIdAsync(registro.Id);
        }

        public async Task<BlacklistDto> UpdateAsync(int id, UpdateBlacklistDto dto)
        {
            var registro = await _context.Blacklist.FindAsync(id);
            if (registro == null)
                return null;

            var (isValid, errorMessage) = await ValidarActualizacionAsync(id, dto);
            if (!isValid)
                throw new InvalidOperationException(errorMessage);

            var valoresAnteriores = System.Text.Json.JsonSerializer.Serialize(new
            {
                registro.Nombre,
                registro.Telefono,
                registro.Email,
                registro.Motivo,
                registro.Notas
            });

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(dto.Nombre))
                registro.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Telefono))
                registro.Telefono = dto.Telefono;
            if (!string.IsNullOrEmpty(dto.Email))
                registro.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Motivo))
                registro.Motivo = dto.Motivo;
            if (!string.IsNullOrEmpty(dto.Notas))
                registro.Notas = dto.Notas;

            await _context.SaveChangesAsync();

            // Registrar en bitácora
            await _bitacoraService.RegistrarAccionAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TipoAccion.MODIFICACION,
                BitacoraEnum.TablaMonitoreo.BLACKLIST,
                registro.Id.ToString(),
                valoresAnteriores,
                System.Text.Json.JsonSerializer.Serialize(dto)
            );

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var registro = await _context.Blacklist.FindAsync(id);
            if (registro == null)
                return false;

            registro.Estado = EstadosEnum.General.INACTIVO;
            await _context.SaveChangesAsync();

            // Registrar en bitácora
            await _bitacoraService.RegistrarCambioEstadoAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TablaMonitoreo.BLACKLIST,
                id.ToString(),
                EstadosEnum.General.ACTIVO,
                EstadosEnum.General.INACTIVO
            );

            return true;
        }

        public async Task<List<BlacklistDto>> GetByFiltroAsync(BlacklistFiltroDto filtro)
        {
            var query = _context.Blacklist
                .Include(b => b.UsuarioRegistro)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtro.Telefono))
                query = query.Where(b => b.Telefono == filtro.Telefono);

            if (!string.IsNullOrEmpty(filtro.Email))
                query = query.Where(b => b.Email == filtro.Email);

            if (filtro.FechaInicio.HasValue)
                query = query.Where(b => b.FechaRegistro >= filtro.FechaInicio.Value);

            if (filtro.FechaFin.HasValue)
                query = query.Where(b => b.FechaRegistro <= filtro.FechaFin.Value);

            if (!string.IsNullOrEmpty(filtro.Estado))
                query = query.Where(b => b.Estado == filtro.Estado);

            var registros = await query
                .OrderByDescending(b => b.FechaRegistro)
                .ToListAsync();

            return registros.Select(MapToDto).ToList();
        }

        public async Task<List<BlacklistDto>> GetActivosAsync()
        {
            var registros = await _context.Blacklist
                .Include(b => b.UsuarioRegistro)
                .Where(b => b.Estado == EstadosEnum.General.ACTIVO)
                .OrderByDescending(b => b.FechaRegistro)
                .ToListAsync();

            return registros.Select(MapToDto).ToList();
        }

        public async Task<BlacklistVerificacionDto> VerificarTelefonoAsync(string telefono)
        {
            var registro = await _context.Blacklist
                .Where(b => b.Telefono == telefono && b.Estado == EstadosEnum.General.ACTIVO)
                .OrderByDescending(b => b.FechaRegistro)
                .FirstOrDefaultAsync();

            return new BlacklistVerificacionDto
            {
                EstaEnBlacklist = registro != null,
                Motivo = registro?.Motivo,
                FechaRegistro = registro?.FechaRegistro
            };
        }

        public async Task<BlacklistVerificacionDto> VerificarEmailAsync(string email)
        {
            var registro = await _context.Blacklist
                .Where(b => b.Email == email && b.Estado == EstadosEnum.General.ACTIVO)
                .OrderByDescending(b => b.FechaRegistro)
                .FirstOrDefaultAsync();

            return new BlacklistVerificacionDto
            {
                EstaEnBlacklist = registro != null,
                Motivo = registro?.Motivo,
                FechaRegistro = registro?.FechaRegistro
            };
        }

        public async Task<bool> ExisteRegistroActivoAsync(string telefono, string email)
        {
            return await _context.Blacklist
                .AnyAsync(b =>
                    b.Estado == EstadosEnum.General.ACTIVO &&
                    ((!string.IsNullOrEmpty(telefono) && b.Telefono == telefono) ||
                     (!string.IsNullOrEmpty(email) && b.Email == email)));
        }

        public async Task<bool> CambiarEstadoAsync(int id, string nuevoEstado, string motivo = null)
        {
            var registro = await _context.Blacklist.FindAsync(id);
            if (registro == null)
                return false;

            var estadoAnterior = registro.Estado;
            registro.Estado = nuevoEstado;

            if (!string.IsNullOrEmpty(motivo))
                registro.Notas = (registro.Notas + "\n" + motivo).Trim();

            await _context.SaveChangesAsync();

            // Registrar en bitácora
            await _bitacoraService.RegistrarCambioEstadoAsync(
                _currentUserService.GetUserId(),
                BitacoraEnum.TablaMonitoreo.BLACKLIST,
                id.ToString(),
                estadoAnterior,
                nuevoEstado,
                motivo
            );

            return true;
        }

        public async Task<Dictionary<string, int>> GetEstadisticasRegistrosAsync(DateTime fechaInicio,DateTime fechaFin)
        {
            var registros = await _context.Blacklist
                .Where(b => b.FechaRegistro >= fechaInicio && b.FechaRegistro <= fechaFin)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                ["TotalRegistros"] = registros.Count,
                ["RegistrosActivos"] = registros.Count(r => r.Estado == EstadosEnum.General.ACTIVO),
                ["RegistrosInactivos"] = registros.Count(r => r.Estado == EstadosEnum.General.INACTIVO)
            };
        }

        public async Task<List<BlacklistDto>> GetUltimosRegistrosAsync(int cantidad = 10)
        {
            var registros = await _context.Blacklist
                .Include(b => b.UsuarioRegistro)
                .OrderByDescending(b => b.FechaRegistro)
                .Take(cantidad)
                .ToListAsync();

            return registros.Select(MapToDto).ToList();
        }

        public async Task<bool> ValidarDatosRegistroAsync(CreateBlacklistDto dto)
        {
            // Validar que al menos uno de los dos campos esté presente
            if (string.IsNullOrEmpty(dto.Telefono) && string.IsNullOrEmpty(dto.Email))
                return false;

            // Validar que no exista un registro activo con el mismo teléfono o email
            return !await ExisteRegistroActivoAsync(dto.Telefono, dto.Email);
        }

        public async Task<(bool isValid, string errorMessage)> ValidarActualizacionAsync(int id,UpdateBlacklistDto dto)
        {
            var registro = await _context.Blacklist.FindAsync(id);
            if (registro == null)
                return (false, "Registro no encontrado");

            // Validar teléfono único si se está actualizando
            if (!string.IsNullOrEmpty(dto.Telefono) && dto.Telefono != registro.Telefono)
            {
                if (await _context.Blacklist.AnyAsync(b =>
                    b.Telefono == dto.Telefono &&
                    b.Id != id &&
                    b.Estado == EstadosEnum.General.ACTIVO))
                {
                    return (false, "Ya existe un registro activo con este teléfono");
                }
            }

            // Validar email único si se está actualizando
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != registro.Email)
            {
                if (await _context.Blacklist.AnyAsync(b =>
                    b.Email == dto.Email &&
                    b.Id != id &&
                    b.Estado == EstadosEnum.General.ACTIVO))
                {
                    return (false, "Ya existe un registro activo con este email");
                }
            }

            return (true, string.Empty);
        }

        private static BlacklistDto MapToDto(BlacklistModel model)
        {
            return new BlacklistDto
            {
                Id = model.Id,
                Nombre = model.Nombre,
                Telefono = model.Telefono,
                Email = model.Email,
                Motivo = model.Motivo,
                FechaRegistro = model.FechaRegistro,
                IdUsuarioRegistro = model.IdUsuarioRegistro,
                NombreUsuarioRegistro = model.UsuarioRegistro?.UserName ?? "Sistema",
                Estado = model.Estado,
                Notas = model.Notas
            };
        }
    }
}
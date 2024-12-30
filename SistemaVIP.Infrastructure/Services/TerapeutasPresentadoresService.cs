using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs.TerapeutaPresentador;
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
    public class TerapeutasPresentadoresService : ITerapeutasPresentadoresService
    {
        private readonly ApplicationDbContext _context;

        public TerapeutasPresentadoresService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TerapeutaPresentadorDto>> GetAllAsync()
        {
            return await _context.TerapeutasPresentadores
                .Include(tp => tp.Terapeuta)
                .Include(tp => tp.Presentador)
                .Select(tp => new TerapeutaPresentadorDto
                {
                    TerapeutaId = tp.TerapeutaId,
                    PresentadorId = tp.PresentadorId,
                    FechaAsignacion = tp.FechaAsignacion,
                    Estado = tp.Estado,
                    NombreTerapeuta = tp.Terapeuta.Nombre,
                    ApellidoTerapeuta = tp.Terapeuta.Apellido,
                    NombrePresentador = tp.Presentador.Nombre,
                    ApellidoPresentador = tp.Presentador.Apellido
                })
                .ToListAsync();
        }

        public async Task<List<TerapeutasPorPresentadorDto>> GetTerapeutasByPresentadorIdAsync(int presentadorId)
        {
            return await _context.TerapeutasPresentadores
                .Include(tp => tp.Terapeuta)
                .Where(tp =>
                    tp.PresentadorId == presentadorId &&
                    tp.Estado == EstadosEnum.General.ACTIVO &&
                    tp.Terapeuta.Estado == EstadosEnum.General.ACTIVO)
                .Select(tp => new TerapeutasPorPresentadorDto
                {
                    TerapeutaId = tp.TerapeutaId,
                    NombreCompleto = $"{tp.Terapeuta.Nombre} {tp.Terapeuta.Apellido}",
                    Telefono = tp.Terapeuta.Telefono,
                    Email = tp.Terapeuta.Email,
                    Estado = tp.Estado,
                    FechaAsignacion = tp.FechaAsignacion
                })
                .ToListAsync();
        }

        public async Task<TerapeutaPresentadorDto> AsignarTerapeutaPresentadorAsync(AsignarTerapeutaPresentadorDto dto)
        {
            // Validar que el terapeuta esté activo
            var terapeuta = await _context.Terapeutas.FindAsync(dto.TerapeutaId);
            if (terapeuta == null || terapeuta.Estado != EstadosEnum.General.ACTIVO)
            {
                throw new InvalidOperationException("El terapeuta no está activo o no existe");
            }

            // Validar que el presentador esté activo
            var presentador = await _context.Presentadores.FindAsync(dto.PresentadorId);
            if (presentador == null || presentador.Estado != EstadosEnum.General.ACTIVO)
            {
                throw new InvalidOperationException("El presentador no está activo o no existe");
            }

            // Verificar si ya existe esta asignación específica
            var asignacionExistente = await _context.TerapeutasPresentadores
                .FirstOrDefaultAsync(tp =>
                    tp.TerapeutaId == dto.TerapeutaId &&
                    tp.PresentadorId == dto.PresentadorId);

            if (asignacionExistente != null)
            {
                throw new InvalidOperationException("Esta terapeuta ya está asignada a este presentador");
            }

            var nuevaAsignacion = new TerapeutasPresentadoresModel
            {
                TerapeutaId = dto.TerapeutaId,
                PresentadorId = dto.PresentadorId,
                FechaAsignacion = DateTime.UtcNow,
                Estado = EstadosEnum.General.ACTIVO
            };

            _context.TerapeutasPresentadores.Add(nuevaAsignacion);
            await _context.SaveChangesAsync();

            var asignacionConDetalles = await _context.TerapeutasPresentadores
                .Include(tp => tp.Terapeuta)
                .Include(tp => tp.Presentador)
                .FirstAsync(tp => tp.TerapeutaId == dto.TerapeutaId && tp.PresentadorId == dto.PresentadorId);

            return MapToDto(asignacionConDetalles);
        }

        public async Task<bool> UpdateEstadoAsync(int terapeutaId, int presentadorId, string estado)
        {
            var asignacion = await _context.TerapeutasPresentadores
                .FindAsync(terapeutaId, presentadorId);

            if (asignacion == null) return false;

            if (!EstadosEnum.EstadosGenerales.Contains(estado))
            {
                throw new InvalidOperationException("Estado no válido");
            }

            asignacion.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsignacionAsync(int terapeutaId, int presentadorId)
        {
            return await _context.TerapeutasPresentadores
                .AnyAsync(tp =>
                    tp.TerapeutaId == terapeutaId &&
                    tp.PresentadorId == presentadorId &&
                    tp.Estado == EstadosEnum.General.ACTIVO);
        }

        private static TerapeutaPresentadorDto MapToDto(TerapeutasPresentadoresModel model)
        {
            return new TerapeutaPresentadorDto
            {
                TerapeutaId = model.TerapeutaId,
                PresentadorId = model.PresentadorId,
                FechaAsignacion = model.FechaAsignacion,
                Estado = model.Estado,
                NombreTerapeuta = model.Terapeuta?.Nombre,
                ApellidoTerapeuta = model.Terapeuta?.Apellido,
                NombrePresentador = model.Presentador?.Nombre,
                ApellidoPresentador = model.Presentador?.Apellido
            };
        }

        public async Task<bool> EliminarAsignacionAsync(int terapeutaId, int presentadorId)
        {
            var asignacion = await _context.TerapeutasPresentadores
                .FirstOrDefaultAsync(tp =>
                    tp.TerapeutaId == terapeutaId &&
                    tp.PresentadorId == presentadorId);

            if (asignacion == null)
                return false;

            _context.TerapeutasPresentadores.Remove(asignacion); // Eliminación física

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
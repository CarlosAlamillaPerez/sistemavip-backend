using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs.TerapeutaPresentador;
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
                .Where(tp => tp.PresentadorId == presentadorId)
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
            // Verificar si el terapeuta ya está asignado a otro presentador
            var asignacionExistente = await _context.TerapeutasPresentadores
                .FirstOrDefaultAsync(tp => tp.TerapeutaId == dto.TerapeutaId && tp.Estado != "Finalizado");

            if (asignacionExistente != null)
            {
                throw new InvalidOperationException($"El terapeuta ya está asignado al presentador {asignacionExistente.PresentadorId}");
            }

            var nuevaAsignacion = new TerapeutasPresentadoresModel
            {
                TerapeutaId = dto.TerapeutaId,
                PresentadorId = dto.PresentadorId,
                FechaAsignacion = DateTime.UtcNow,
                Estado = "Pendiente"
            };

            _context.TerapeutasPresentadores.Add(nuevaAsignacion);
            await _context.SaveChangesAsync();

            // Cargar los datos relacionados para el DTO de respuesta
            var asignacionConDetalles = await _context.TerapeutasPresentadores
                .Include(tp => tp.Terapeuta)
                .Include(tp => tp.Presentador)
                .FirstAsync(tp => tp.TerapeutaId == dto.TerapeutaId && tp.PresentadorId == dto.PresentadorId);

            return new TerapeutaPresentadorDto
            {
                TerapeutaId = asignacionConDetalles.TerapeutaId,
                PresentadorId = asignacionConDetalles.PresentadorId,
                FechaAsignacion = asignacionConDetalles.FechaAsignacion,
                Estado = asignacionConDetalles.Estado,
                NombreTerapeuta = asignacionConDetalles.Terapeuta.Nombre,
                ApellidoTerapeuta = asignacionConDetalles.Terapeuta.Apellido,
                NombrePresentador = asignacionConDetalles.Presentador.Nombre,
                ApellidoPresentador = asignacionConDetalles.Presentador.Apellido
            };
        }

        public async Task<bool> UpdateEstadoAsync(int terapeutaId, int presentadorId, string estado)
        {
            var asignacion = await _context.TerapeutasPresentadores
                .FindAsync(terapeutaId, presentadorId);

            if (asignacion == null) return false;

            if (!new[] { "Pendiente", "Confirmado", "EnProceso", "Finalizado", "Cancelado", "NoRealizado" }
                .Contains(estado))
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
                .AnyAsync(tp => tp.TerapeutaId == terapeutaId &&
                               tp.PresentadorId == presentadorId &&
                               tp.Estado != "Finalizado");
        }
    }
}
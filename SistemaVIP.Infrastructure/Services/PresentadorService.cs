using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs.Presentador;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class PresentadorService : IPresentadorService
    {
        private readonly ApplicationDbContext _context;

        public PresentadorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PresentadorDto>> GetAllAsync()
        {
            var presentadores = await _context.Presentadores
                .AsNoTracking()
                .OrderByDescending(p => p.FechaAlta)
                .Select(p => MapToDto(p))
                .ToListAsync();

            return presentadores;
        }

        public async Task<PresentadorDto> GetByIdAsync(int id)
        {
            var presentador = await _context.Presentadores
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return presentador != null ? MapToDto(presentador) : null;
        }

        public async Task<PresentadorDto> GetByUserIdAsync(string userId)
        {
            var presentador = await _context.Presentadores
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return presentador != null ? MapToDto(presentador) : null;
        }

        public async Task<PresentadorDto> CreateAsync(CreatePresentadorDto dto)
        {
            // Validar que no exista otro presentador con el mismo email o documento
            if (await _context.Presentadores.AnyAsync(p =>
                p.Email == dto.Email ||
                p.DocumentoIdentidad == dto.DocumentoIdentidad))
            {
                throw new InvalidOperationException("Ya existe un presentador con el mismo email o documento de identidad.");
            }

            var presentador = new PresentadorModel
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Email = dto.Email,
                PorcentajeComision = dto.PorcentajeComision,
                DocumentoIdentidad = dto.DocumentoIdentidad,
                FotoUrl = dto.FotoUrl,
                Notas = dto.Notas,
                FechaAlta = DateTime.UtcNow,
                UltimaActualizacion = DateTime.UtcNow,
                Estado = "Activo"
            };

            _context.Presentadores.Add(presentador);
            await _context.SaveChangesAsync();

            return MapToDto(presentador);
        }

        public async Task<PresentadorDto> UpdateAsync(int id, UpdatePresentadorDto dto)
        {
            var presentador = await _context.Presentadores.FindAsync(id);
            if (presentador == null)
                return null;

            // Validar email único si se está actualizando
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != presentador.Email)
            {
                if (await _context.Presentadores.AnyAsync(p => p.Email == dto.Email && p.Id != id))
                {
                    throw new InvalidOperationException("El email ya está en uso por otro presentador.");
                }
            }

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(dto.Nombre))
                presentador.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido))
                presentador.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.Telefono))
                presentador.Telefono = dto.Telefono;
            if (!string.IsNullOrEmpty(dto.Email))
                presentador.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.FotoUrl))
                presentador.FotoUrl = dto.FotoUrl;
            if (!string.IsNullOrEmpty(dto.Notas))
                presentador.Notas = dto.Notas;

            presentador.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(presentador);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var presentador = await _context.Presentadores.FindAsync(id);
            if (presentador == null)
                return false;

            presentador.Estado = "Inactivo";
            presentador.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEstadoAsync(int id, string estado)
        {
            var presentador = await _context.Presentadores.FindAsync(id);
            if (presentador == null)
                return false;

            presentador.Estado = estado;
            presentador.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PresentadorDto>> GetActivosAsync()
        {
            var presentadores = await _context.Presentadores
                .AsNoTracking()
                .Where(p => p.Estado == "Activo")
                .OrderByDescending(p => p.FechaAlta)
                .Select(p => MapToDto(p))
                .ToListAsync();

            return presentadores;
        }

        public async Task<bool> UpdateComisionAsync(int id, decimal nuevoPorcentaje)
        {
            var presentador = await _context.Presentadores.FindAsync(id);
            if (presentador == null)
                return false;

            presentador.PorcentajeComision = nuevoPorcentaje;
            presentador.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private static PresentadorDto MapToDto(PresentadorModel model)
        {
            return new PresentadorDto
            {
                Id = model.Id,
                UserId = model.UserId,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Telefono = model.Telefono,
                Email = model.Email,
                PorcentajeComision = model.PorcentajeComision,
                FechaAlta = model.FechaAlta,
                Estado = model.Estado,
                DocumentoIdentidad = model.DocumentoIdentidad,
                FotoUrl = model.FotoUrl,
                UltimaActualizacion = model.UltimaActualizacion,
                Notas = model.Notas
            };
        }
    }
}
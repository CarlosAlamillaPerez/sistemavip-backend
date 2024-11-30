using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs.Terapeuta;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class TerapeutaService : ITerapeutaService
    {
        private readonly ApplicationDbContext _context;

        public TerapeutaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TerapeutaDto>> GetAllAsync()
        {
            var terapeutas = await _context.Terapeutas
                .AsNoTracking()
                .OrderByDescending(t => t.FechaAlta)
                .Select(t => MapToDto(t))
                .ToListAsync();

            return terapeutas;
        }

        public async Task<TerapeutaDto> GetByIdAsync(int id)
        {
            var terapeuta = await _context.Terapeutas
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            return terapeuta != null ? MapToDto(terapeuta) : null;
        }

        public async Task<TerapeutaDto> GetByUserIdAsync(string userId)
        {
            var terapeuta = await _context.Terapeutas
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserId == userId);

            return terapeuta != null ? MapToDto(terapeuta) : null;
        }

        public async Task<TerapeutaDto> CreateAsync(CreateTerapeutaDto dto)
        {
            // Validar que no exista otro terapeuta con el mismo email o documento
            if (await _context.Terapeutas.AnyAsync(t =>
                t.Email == dto.Email ||
                t.DocumentoIdentidad == dto.DocumentoIdentidad))
            {
                throw new InvalidOperationException("Ya existe un terapeuta con el mismo email o documento de identidad.");
            }

            var terapeuta = new TerapeutaModel
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Email = dto.Email,
                FechaNacimiento = dto.FechaNacimiento,
                Estatura = dto.Estatura,
                DocumentoIdentidad = dto.DocumentoIdentidad,
                FotoUrl = dto.FotoUrl,
                Notas = dto.Notas,
                TarifaBase = dto.TarifaBase,
                TarifaExtra = dto.TarifaExtra,
                FechaAlta = DateTime.UtcNow,
                UltimaActualizacion = DateTime.UtcNow,
                Estado = "Activo"
            };

            _context.Terapeutas.Add(terapeuta);
            await _context.SaveChangesAsync();

            return MapToDto(terapeuta);
        }

        public async Task<TerapeutaDto> UpdateAsync(int id, UpdateTerapeutaDto dto)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);
            if (terapeuta == null)
                return null;

            // Validar email único si se está actualizando
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != terapeuta.Email)
            {
                if (await _context.Terapeutas.AnyAsync(t => t.Email == dto.Email && t.Id != id))
                {
                    throw new InvalidOperationException("El email ya está en uso por otro terapeuta.");
                }
            }

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(dto.Nombre))
                terapeuta.Nombre = dto.Nombre;
            if (!string.IsNullOrEmpty(dto.Apellido))
                terapeuta.Apellido = dto.Apellido;
            if (!string.IsNullOrEmpty(dto.Telefono))
                terapeuta.Telefono = dto.Telefono;
            if (!string.IsNullOrEmpty(dto.Email))
                terapeuta.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Estatura))
                terapeuta.Estatura = dto.Estatura;
            if (!string.IsNullOrEmpty(dto.FotoUrl))
                terapeuta.FotoUrl = dto.FotoUrl;
            if (!string.IsNullOrEmpty(dto.Notas))
                terapeuta.Notas = dto.Notas;

            terapeuta.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(terapeuta);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);
            if (terapeuta == null)
                return false;

            terapeuta.Estado = "Inactivo";
            terapeuta.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEstadoAsync(int id, string estado)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);
            if (terapeuta == null)
                return false;

            terapeuta.Estado = estado;
            terapeuta.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TerapeutaDto>> GetActivosAsync()
        {
            var terapeutas = await _context.Terapeutas
                .AsNoTracking()
                .Where(t => t.Estado == "Activo")
                .OrderByDescending(t => t.FechaAlta)
                .Select(t => MapToDto(t))
                .ToListAsync();

            return terapeutas;
        }

        public async Task<bool> UpdateTarifasAsync(int id, decimal tarifaBase, decimal tarifaExtra)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);
            if (terapeuta == null)
                return false;

            terapeuta.TarifaBase = tarifaBase;
            terapeuta.TarifaExtra = tarifaExtra;
            terapeuta.UltimaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private static TerapeutaDto MapToDto(TerapeutaModel model)
        {
            return new TerapeutaDto
            {
                Id = model.Id,
                UserId = model.UserId,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Telefono = model.Telefono,
                Email = model.Email,
                FechaNacimiento = model.FechaNacimiento,
                FechaAlta = model.FechaAlta,
                Estado = model.Estado,
                Estatura = model.Estatura,
                DocumentoIdentidad = model.DocumentoIdentidad,
                FotoUrl = model.FotoUrl,
                UltimaActualizacion = model.UltimaActualizacion,
                Notas = model.Notas,
                TarifaBase = model.TarifaBase,
                TarifaExtra = model.TarifaExtra
            };
        }
    }
}
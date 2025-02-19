﻿using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Servicio;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class ServicioExtraService : IServicioExtraService
    {
        private readonly ApplicationDbContext _context;

        public ServicioExtraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServicioExtraCatalogoDto>> GetCatalogoActivoAsync()
        {
            return await _context.ServiciosExtraCatalogo
                .Select(s => new ServicioExtraCatalogoDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Estado = s.Estado
                })
                .ToListAsync();
        }

        public async Task<List<ServicioExtraDetalleDto>> GetServiciosExtraByServicioAsync(int servicioTerapeutaId)
        {
            return await _context.ServiciosExtra
                .Include(se => se.ServicioExtraCatalogo)
                .Where(se => se.ServicioTerapeutaId == servicioTerapeutaId)
                .Select(se => new ServicioExtraDetalleDto
                {
                    Id = se.Id,
                    ServicioExtraCatalogoId = se.ServicioExtraCatalogoId,
                    NombreServicio = se.ServicioExtraCatalogo.Nombre,
                    Monto = se.Monto,
                    FechaRegistro = se.FechaRegistro,
                    Notas = se.Notas
                })
                .ToListAsync();
        }

        public async Task<bool> AgregarServiciosExtraAsync(int servicioTerapeutaId, CreateServicioExtraDto dto)
        {
            var servicioTerapeuta = await _context.ServiciosTerapeutas
                .Include(st => st.ServiciosExtra)
                .FirstOrDefaultAsync(st => st.Id == servicioTerapeutaId);

            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio no encontrado");

            if (servicioTerapeuta.Estado == "PAGADO")
                throw new InvalidOperationException("No se pueden agregar servicios extra a un servicio pagado");

            if (servicioTerapeuta.Estado == "CANCELADO")
                throw new InvalidOperationException("No se pueden agregar servicios extra a un servicio cancelado");

            foreach (var item in dto.ServiciosExtra)
            {
                // Validar que el servicio extra existe y está activo
                var servicioExtraCatalogo = await _context.ServiciosExtraCatalogo
                    .FirstOrDefaultAsync(s => s.Id == item.ServicioExtraCatalogoId && s.Estado);

                if (servicioExtraCatalogo == null)
                    throw new InvalidOperationException($"Servicio extra con ID {item.ServicioExtraCatalogoId} no existe o no está activo");

                if (item.Monto <= 0)
                    throw new InvalidOperationException("El monto debe ser mayor a 0");

                var servicioExtra = new ServicioExtraModel
                {
                    ServicioTerapeutaId = servicioTerapeutaId,
                    ServicioExtraCatalogoId = item.ServicioExtraCatalogoId,
                    Monto = item.Monto,
                    FechaRegistro = DateTime.UtcNow,
                    Notas = item.Notas
                };

                _context.ServiciosExtra.Add(servicioExtra);
            }

            await _context.SaveChangesAsync();
            return true;
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
        public async Task<ServicioExtraCatalogoDto> CreateServicioExtraCatalogoAsync(CreateServicioExtraCatalogoDto dto)
        {
            // Validar que no exista un servicio con el mismo nombre
            var existeServicio = await _context.ServiciosExtraCatalogo
                .AnyAsync(s => s.Nombre.ToLower() == dto.Nombre.ToLower());

            if (existeServicio)
                throw new InvalidOperationException("Ya existe un servicio extra con este nombre");

            var servicioExtra = new ServicioExtraCatalogoModel
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado
            };

            _context.ServiciosExtraCatalogo.Add(servicioExtra);
            await _context.SaveChangesAsync();

            return new ServicioExtraCatalogoDto
            {
                Id = servicioExtra.Id,
                Nombre = servicioExtra.Nombre,
                Descripcion = servicioExtra.Descripcion,
                Estado = servicioExtra.Estado
            };
        }
        public async Task<ServicioExtraCatalogoDto> UpdateServicioExtraCatalogoAsync(int id, UpdateServicioExtraCatalogoDto dto)
        {
            var servicioExtra = await _context.ServiciosExtraCatalogo
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servicioExtra == null)
                throw new InvalidOperationException("Servicio extra no encontrado");

            // Verificar si existe otro servicio con el mismo nombre (excluyendo el actual)
            var existeServicio = await _context.ServiciosExtraCatalogo
                .AnyAsync(s => s.Id != id && s.Nombre.ToLower() == dto.Nombre.ToLower());

            if (existeServicio)
                throw new InvalidOperationException("Ya existe otro servicio extra con este nombre");

            servicioExtra.Nombre = dto.Nombre;
            servicioExtra.Descripcion = dto.Descripcion;
            servicioExtra.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            return new ServicioExtraCatalogoDto
            {
                Id = servicioExtra.Id,
                Nombre = servicioExtra.Nombre,
                Descripcion = servicioExtra.Descripcion,
                Estado = servicioExtra.Estado
            };
        }
        public async Task<bool> DeleteServicioExtraCatalogoAsync(int id)
        {
            var servicioExtra = await _context.ServiciosExtraCatalogo
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servicioExtra == null)
                return false;

            // Verificar si el servicio está siendo utilizado
            var tieneServiciosAsociados = await _context.ServiciosExtra
                .AnyAsync(se => se.ServicioExtraCatalogoId == id);

            if (tieneServiciosAsociados)
                throw new InvalidOperationException("No se puede eliminar el servicio extra porque está siendo utilizado en servicios existentes");

            _context.ServiciosExtraCatalogo.Remove(servicioExtra);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<ServicioExtraCatalogoDto> GetServicioExtraCatalogoByIdAsync(int id)
        {
            var servicioExtra = await _context.ServiciosExtraCatalogo
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servicioExtra == null)
                return null;

            return new ServicioExtraCatalogoDto
            {
                Id = servicioExtra.Id,
                Nombre = servicioExtra.Nombre,
                Descripcion = servicioExtra.Descripcion,
                Estado = servicioExtra.Estado
            };
        }

    }
}
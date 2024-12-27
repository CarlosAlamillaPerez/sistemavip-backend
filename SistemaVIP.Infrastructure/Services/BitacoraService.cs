using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly ApplicationDbContext _context;

        public BitacoraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegistrarAccionAsync(
            string idUsuario,
            string accion,
            string tabla,
            string idRegistro,
            string valoresAnteriores = null,
            string valoresNuevos = null)
        {
            if (!BitacoraEnum.TiposAccion.Contains(accion))
                throw new InvalidOperationException("Tipo de acción no válido");

            if (!BitacoraEnum.TablasMonitoreo.Contains(tabla))
                throw new InvalidOperationException("Tabla no válida para monitoreo");

            var registro = new BitacoraModel
            {
                Fecha = DateTime.UtcNow,
                IdUsuario = idUsuario,
                Accion = accion,
                Tabla = tabla,
                IdRegistro = idRegistro,
                ValoresAnteriores = valoresAnteriores,
                ValoresNuevos = valoresNuevos
            };

            _context.Bitacora.Add(registro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RegistrarCambioEstadoAsync(
            string idUsuario,
            string tabla,
            string idRegistro,
            string estadoAnterior,
            string estadoNuevo,
            string motivo = null)
        {
            var valoresAnteriores = new { Estado = estadoAnterior };
            var valoresNuevos = new { Estado = estadoNuevo, Motivo = motivo };

            return await RegistrarAccionAsync(
                idUsuario,
                BitacoraEnum.TipoAccion.CAMBIO_ESTADO,
                tabla,
                idRegistro,
                JsonSerializer.Serialize(valoresAnteriores),
                JsonSerializer.Serialize(valoresNuevos)
            );
        }

        public async Task<bool> RegistrarValidacionAsync(
            string idUsuario,
            string tabla,
            string idRegistro,
            bool exitosa,
            string detalles)
        {
            var valores = new { Exitosa = exitosa, Detalles = detalles };

            return await RegistrarAccionAsync(
                idUsuario,
                BitacoraEnum.TipoAccion.VALIDACION,
                tabla,
                idRegistro,
                null,
                JsonSerializer.Serialize(valores)
            );
        }

        public async Task<List<BitacoraDto>> GetByRegistroAsync(string tabla, string idRegistro)
        {
            var registros = await _context.Bitacora
                .Include(b => b.Usuario)
                .Where(b => b.Tabla == tabla && b.IdRegistro == idRegistro)
                .OrderByDescending(b => b.Fecha)
                .ToListAsync();

            return registros.Select(MapToDto).ToList();
        }

        public async Task<List<BitacoraDto>> GetByFiltroAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            string tabla = null,
            string idUsuario = null)
        {
            var query = _context.Bitacora
                .Include(b => b.Usuario)
                .Where(b => b.Fecha >= fechaInicio && b.Fecha <= fechaFin);

            if (!string.IsNullOrEmpty(tabla))
                query = query.Where(b => b.Tabla == tabla);

            if (!string.IsNullOrEmpty(idUsuario))
                query = query.Where(b => b.IdUsuario == idUsuario);

            var registros = await query
                .OrderByDescending(b => b.Fecha)
                .ToListAsync();

            return registros.Select(MapToDto).ToList();
        }

        private static BitacoraDto MapToDto(BitacoraModel model)
        {
            return new BitacoraDto
            {
                Id = model.Id,
                Fecha = model.Fecha,
                IdUsuario = model.IdUsuario,
                NombreUsuario = model.Usuario?.UserName ?? "Sistema",
                Accion = model.Accion,
                Tabla = model.Tabla,
                IdRegistro = model.IdRegistro,
                ValoresAnteriores = model.ValoresAnteriores,
                ValoresNuevos = model.ValoresNuevos
            };
        }
    }
}
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
    public class ComisionService : IComisionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly decimal _porcentajeEmpresa = 0.70m;
        private readonly decimal _porcentajePresentador = 0.30m;

        public ComisionService(
            ApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<ComisionDto>> GetAllAsync()
        {
            var comisiones = await _context.Comisiones
                .Include(c => c.Presentador)
                .Include(c => c.Terapeuta)
                .Include(c => c.UsuarioConfirmacion)
                .Include(c => c.UsuarioLiquidacion)
                .OrderByDescending(c => c.FechaCalculo)
                .ToListAsync();

            return comisiones.Select(MapToDto).ToList();
        }

        public async Task<ComisionDto> GetByIdAsync(int id)
        {
            var comision = await _context.Comisiones
                .Include(c => c.Presentador)
                .Include(c => c.Terapeuta)
                .Include(c => c.UsuarioConfirmacion)
                .Include(c => c.UsuarioLiquidacion)
                .FirstOrDefaultAsync(c => c.Id == id);

            return comision != null ? MapToDto(comision) : null;
        }

        public async Task<List<ComisionDto>> GetByFiltroAsync(FiltroComisionesDto filtro)
        {
            var query = _context.Comisiones
                .Include(c => c.Presentador)
                .Include(c => c.Terapeuta)
                .Include(c => c.UsuarioConfirmacion)
                .Include(c => c.UsuarioLiquidacion)
                .AsQueryable();

            if (filtro.FechaInicio.HasValue)
                query = query.Where(c => c.FechaCalculo >= filtro.FechaInicio.Value);

            if (filtro.FechaFin.HasValue)
                query = query.Where(c => c.FechaCalculo <= filtro.FechaFin.Value);

            if (filtro.PresentadorId.HasValue)
                query = query.Where(c => c.PresentadorId == filtro.PresentadorId.Value);

            if (filtro.TerapeutaId.HasValue)
                query = query.Where(c => c.TerapeutaId == filtro.TerapeutaId.Value);

            if (!string.IsNullOrEmpty(filtro.Estado))
                query = query.Where(c => c.Estado == filtro.Estado);

            var comisiones = await query.OrderByDescending(c => c.FechaCalculo).ToListAsync();
            return comisiones.Select(MapToDto).ToList();
        }

        public async Task<ComisionDto> CalcularComisionServicioAsync(int servicioId)
        {
            var servicio = await _context.Servicios
                .Include(s => s.ServiciosTerapeutas)
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
                throw new InvalidOperationException("Servicio no encontrado");

            var servicioTerapeuta = servicio.ServiciosTerapeutas.FirstOrDefault();
            if (servicioTerapeuta == null)
                throw new InvalidOperationException("Servicio sin terapeuta asignado");

            var montoBaseComisiones = servicio.MontoTotal - servicioTerapeuta.MontoTerapeuta.Value;
            var comisionEmpresa = montoBaseComisiones * _porcentajeEmpresa;
            var comisionPresentador = montoBaseComisiones * _porcentajePresentador;

            var comision = new ComisionesModel
            {
                ServicioId = servicioId,
                TerapeutaId = servicioTerapeuta.TerapeutaId,
                PresentadorId = servicio.PresentadorId,
                MontoTotal = servicio.MontoTotal,
                MontoTerapeuta = servicioTerapeuta.MontoTerapeuta.Value,
                MontoComisionTotal = montoBaseComisiones,
                MontoComisionEmpresa = comisionEmpresa,
                MontoComisionPresentador = comisionPresentador,
                PorcentajeAplicadoEmpresa = _porcentajeEmpresa * 100,
                PorcentajeAplicadoPresentador = _porcentajePresentador * 100,
                FechaCalculo = DateTime.UtcNow,
                Estado = EstadosEnum.Comision.NO_PAGADO
            };

            _context.Comisiones.Add(comision);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(comision.Id);
        }

        public async Task<List<ResumenComisionesDto>> GetResumenPresentadoresAsync()
        {
            var presentadores = await _context.Presentadores
                .Where(p => p.Estado == EstadosEnum.General.ACTIVO)
                .Select(p => new ResumenComisionesDto
                {
                    PresentadorId = p.Id,
                    NombrePresentador = $"{p.Nombre} {p.Apellido}",
                    TotalPendientePago = _context.Comisiones
                        .Where(c => c.PresentadorId == p.Id && c.Estado == EstadosEnum.Comision.NO_PAGADO)
                        .Sum(c => c.MontoComisionPresentador),
                    TotalPorConfirmar = _context.Comisiones
                        .Where(c => c.PresentadorId == p.Id && c.Estado == EstadosEnum.Comision.POR_CONFIRMAR)
                        .Sum(c => c.MontoComisionPresentador),
                    TotalConfirmado = _context.Comisiones
                        .Where(c => c.PresentadorId == p.Id && c.Estado == EstadosEnum.Comision.PAGADO)
                        .Sum(c => c.MontoComisionPresentador),
                    CantidadServiciosPendientes = _context.Comisiones
                        .Count(c => c.PresentadorId == p.Id && c.Estado == EstadosEnum.Comision.NO_PAGADO),
                    CantidadServiciosPorConfirmar = _context.Comisiones
                        .Count(c => c.PresentadorId == p.Id && c.Estado == EstadosEnum.Comision.POR_CONFIRMAR),
                    CantidadServiciosConfirmados = _context.Comisiones
                        .Count(c => c.PresentadorId == p.Id && c.Estado == EstadosEnum.Comision.PAGADO),
                    UltimaLiquidacion = _context.Comisiones
                        .Where(c => c.PresentadorId == p.Id && c.Estado == EstadosEnum.Comision.LIQUIDADO)
                        .Max(c => c.FechaLiquidacionPresentador)
                }).ToListAsync();

            return presentadores;
        }

        public async Task<ResumenComisionesDto> GetResumenPresentadorAsync(int presentadorId)
        {
            var presentador = await _context.Presentadores
                .FirstOrDefaultAsync(p => p.Id == presentadorId);

            if (presentador == null)
                throw new InvalidOperationException("Presentador no encontrado");

            return new ResumenComisionesDto
            {
                PresentadorId = presentador.Id,
                NombrePresentador = $"{presentador.Nombre} {presentador.Apellido}",
                TotalPendientePago = await _context.Comisiones
                    .Where(c => c.PresentadorId == presentadorId && c.Estado == EstadosEnum.Comision.NO_PAGADO)
                    .SumAsync(c => c.MontoComisionPresentador),
                TotalPorConfirmar = await _context.Comisiones
                    .Where(c => c.PresentadorId == presentadorId && c.Estado == EstadosEnum.Comision.POR_CONFIRMAR)
                    .SumAsync(c => c.MontoComisionPresentador),
                TotalConfirmado = await _context.Comisiones
                    .Where(c => c.PresentadorId == presentadorId && c.Estado == EstadosEnum.Comision.PAGADO)
                    .SumAsync(c => c.MontoComisionPresentador),
                CantidadServiciosPendientes = await _context.Comisiones
                    .CountAsync(c => c.PresentadorId == presentadorId && c.Estado == EstadosEnum.Comision.NO_PAGADO),
                CantidadServiciosPorConfirmar = await _context.Comisiones
                    .CountAsync(c => c.PresentadorId == presentadorId && c.Estado == EstadosEnum.Comision.POR_CONFIRMAR),
                CantidadServiciosConfirmados = await _context.Comisiones
                    .CountAsync(c => c.PresentadorId == presentadorId && c.Estado == EstadosEnum.Comision.PAGADO),
                UltimaLiquidacion = await _context.Comisiones
                    .Where(c => c.PresentadorId == presentadorId && c.Estado == EstadosEnum.Comision.LIQUIDADO)
                    .MaxAsync(c => c.FechaLiquidacionPresentador)
            };
        }

        public async Task<ComisionDto> RegistrarPagoTerapeutaAsync(int comisionId, RegistroPagoComisionDto dto)
        {
            var comision = await _context.Comisiones
                .Include(c => c.Servicio)
                .FirstOrDefaultAsync(c => c.Id == comisionId);

            if (comision == null)
                throw new InvalidOperationException("Comisión no encontrada");

            if (comision.Estado != EstadosEnum.Comision.NO_PAGADO &&
                comision.Estado != EstadosEnum.Comision.PAGADO)
                throw new InvalidOperationException("La comisión no está en estado pendiente de pago");

            comision.Estado = EstadosEnum.Comision.LIQUIDADO;  // Cambiado de POR_CONFIRMAR a LIQUIDADO
            comision.NumeroTransaccion = dto.NumeroTransaccion;
            comision.ComprobanteUrl = dto.ComprobanteUrl;
            comision.NotasPago = dto.NotasPago;
            comision.FechaPagoTerapeuta = DateTime.UtcNow;
            comision.FechaLiquidacionPresentador = DateTime.UtcNow;  // Agregado
            comision.IdUsuarioConfirmacion = _currentUserService.GetUserId();  // Agregado
            comision.FechaConfirmacion = DateTime.UtcNow;  // Agregado
            comision.IdUsuarioLiquidacion = _currentUserService.GetUserId();  // Agregado
            comision.FechaLiquidacion = DateTime.UtcNow;  // Agregado
            comision.Servicio.Estado = EstadosEnum.Servicio.LIQUIDADO;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(comisionId);
        }

        public async Task<List<ComisionDto>> GetComisionesPendientesLiquidacionAsync(int presentadorId)
        {
            var comisiones = await _context.Comisiones
                .Include(c => c.Presentador)
                .Include(c => c.Terapeuta)
                .Include(c => c.Servicio)
                .Where(c => c.PresentadorId == presentadorId &&
                            c.Estado == EstadosEnum.Comision.PAGADO)
                .OrderBy(c => c.FechaCalculo)
                .ToListAsync();

            return comisiones.Select(MapToDto).ToList();
        }

        public async Task<ComisionDto> CambiarEstadoPagoAsync(int comisionId, string nuevoEstado, string? notas = null)
        {
            var comision = await _context.Comisiones.FindAsync(comisionId);
            if (comision == null)
                throw new InvalidOperationException("Comisión no encontrada");

            if (!EstadosEnum.EstadosComision.Contains(nuevoEstado))
                throw new InvalidOperationException("Estado no válido");

            // Solo permitir cambio a PAGADO si está en POR_CONFIRMAR
            if (nuevoEstado == EstadosEnum.Comision.PAGADO &&
                comision.Estado != EstadosEnum.Comision.POR_CONFIRMAR)
            {
                throw new InvalidOperationException("Solo se puede cambiar a PAGADO desde el estado POR_CONFIRMAR");
            }

            var estadoAnterior = comision.Estado;
            comision.Estado = nuevoEstado;

            if (nuevoEstado == EstadosEnum.Comision.PAGADO)
            {
                comision.IdUsuarioConfirmacion = _currentUserService.GetUserId();
                comision.FechaConfirmacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(comisionId);
        }

        public async Task<List<ComisionDto>> GetComisionesLiquidadasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var comisiones = await _context.Comisiones
                .Include(c => c.Presentador)
                .Include(c => c.Terapeuta)
                .Include(c => c.Servicio)
                .Where(c => c.Estado == EstadosEnum.Comision.LIQUIDADO &&
                            c.FechaLiquidacion >= fechaInicio &&
                            c.FechaLiquidacion <= fechaFin)
                .OrderByDescending(c => c.FechaLiquidacion)
                .ToListAsync();

            return comisiones.Select(MapToDto).ToList();
        }

        private static ComisionDto MapToDto(ComisionesModel model)
        {
            return new ComisionDto
            {
                Id = model.Id,
                ServicioId = model.ServicioId,
                TerapeutaId = model.TerapeutaId,
                PresentadorId = model.PresentadorId,
                MontoTotal = model.MontoTotal,
                MontoTerapeuta = model.MontoTerapeuta,
                MontoComisionTotal = model.MontoComisionTotal,
                MontoComisionEmpresa = model.MontoComisionEmpresa,
                MontoComisionPresentador = model.MontoComisionPresentador,
                PorcentajeAplicadoEmpresa = model.PorcentajeAplicadoEmpresa,
                PorcentajeAplicadoPresentador = model.PorcentajeAplicadoPresentador,
                FechaCalculo = model.FechaCalculo,
                Estado = model.Estado,
                NumeroTransaccion = model.NumeroTransaccion,
                ComprobanteUrl = model.ComprobanteUrl,
                FechaPagoTerapeuta = model.FechaPagoTerapeuta,
                FechaLiquidacionPresentador = model.FechaLiquidacionPresentador,
                NotasPago = model.NotasPago,
                NombrePresentador = $"{model.Presentador?.Nombre} {model.Presentador?.Apellido}".Trim(),
                NombreTerapeuta = $"{model.Terapeuta?.Nombre} {model.Terapeuta?.Apellido}".Trim(),
                IdUsuarioConfirmacion = model.IdUsuarioConfirmacion,
                NombreUsuarioConfirmacion = $"{model.UsuarioConfirmacion?.UserName}",
                FechaConfirmacion = model.FechaConfirmacion,
                IdUsuarioLiquidacion = model.IdUsuarioLiquidacion,
                NombreUsuarioLiquidacion = $"{model.UsuarioLiquidacion?.UserName}",
                FechaLiquidacion = model.FechaLiquidacion
            };
        }
    }
}
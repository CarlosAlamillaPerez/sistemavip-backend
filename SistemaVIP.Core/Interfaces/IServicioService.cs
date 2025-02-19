﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Servicio;

namespace SistemaVIP.Core.Interfaces
{
    public interface IServicioService
    {
        // CRUD básico
        Task<List<ServicioDto>> GetAllAsync();
        Task<ServicioDto> GetByIdAsync(int id);
        Task<ServicioDto> CreateAsync(CreateServicioDto createDto);
        Task<ServicioDto> UpdateAsync(int id, UpdateServicioDto updateDto);
        //Task<List<ServicioDto>> GetServiciosActivosByPresentadorAsync(int presentadorId);
        //Task<List<ServicioDto>> GetServiciosActivosByTerapeutaAsync(int terapeutaId);

        // Operaciones específicas de servicio
        Task<ServicioDto> CancelarServicioAsync(int id, CancelacionServicioDto cancelacionDto, string userId);
        Task<object> GetServicioTerapeutaByLinkConfirmacionAsync(Guid linkConfirmacion, bool detalleCompleto = false);
        Task<ServicioTerapeutaDto> GetServicioTerapeutaByLinkFinalizacionAsync(Guid linkFinalizacion);


        // Confirmación y finalización de servicios
        Task<ServicioTerapeutaDto> ConfirmarInicioServicioAsync(ConfirmacionServicioDto confirmacionDto);
        Task<ServicioTerapeutaDto> FinalizarServicioAsync(FinalizacionServicioDto finalizacionDto);

        // Consultas filtradas
        Task<List<ServicioDto>> GetServiciosByPresentadorAsync(int presentadorId);
        Task<List<ServicioDto>> GetServiciosByTerapeutaAsync(int terapeutaId);
        Task<List<ServicioDto>> GetServiciosByFechaAsync(DateTime fecha);
        Task<List<ServicioDto>> GetServiciosActivosAsync();

        // Validaciones
        Task<bool> ValidateServicioTerapeutasAsync(List<CreateServicioTerapeutaDto> terapeutas, int presentadorId);
        Task<bool> ValidateLocationDistanceAsync(double startLat, double startLng, double endLat, double endLng);
        Task<ServicioTerapeutaDto> AgregarComprobantePagoAsync(int servicioTerapeutaId, CreateComprobantePagoDto dto);
        Task<ServicioTerapeutaDto> ActualizarEstadoComprobanteAsync(int servicioTerapeutaId, int comprobanteId, UpdateComprobanteEstadoDto dto);
        Task<ConciliacionServicioDto> GetConciliacionServicioAsync(int servicioTerapeutaId);
        Task<ResultadoConciliacionDto> RealizarConciliacionAsync(int servicioTerapeutaId);
        Task<bool> ValidarConciliacionAsync(int servicioTerapeutaId);
        Task NotificarCancelacionesExcesivasAsync();
        Task<ServicioTerapeutaDto> AgregarComprobantesMultiplesAsync(int servicioTerapeutaId, CreateComprobantesMultiplesDto dto);
        Task EliminarComprobantePagoAsync(int servicioTerapeutaId, int comprobanteId);
        Task<List<ComprobantePagoDto>> GetComprobantesPagoByServicioAsync(int servicioTerapeutaId);
        Task<bool> DeleteServicioAsync(int id);
        Task<ServicioDto> CancelarServicioAsync(int id, CancelacionServicioDto dto);
    }
}

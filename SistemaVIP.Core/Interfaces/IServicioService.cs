using System;
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
        Task<ServicioTerapeutaDto> GetServicioTerapeutaByLinkConfirmacionAsync(Guid linkConfirmacion);
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
    }
}
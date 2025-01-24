using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.DTOs.Servicio;

namespace SistemaVIP.Core.Interfaces
{
    public interface IServicioExtraService
    {
        Task<List<ServicioExtraCatalogoDto>> GetCatalogoActivoAsync();
        Task<bool> AgregarServiciosExtraAsync(int servicioTerapeutaId, CreateServicioExtraDto dto);
        Task<List<ServicioExtraDetalleDto>> GetServiciosExtraByServicioAsync(int servicioTerapeutaId);
        Task<ServicioExtraDetalleDto> UpdateServicioExtraAsync(int servicioTerapeutaId, int servicioExtraId, UpdateServicioExtraDto dto);
        Task<bool> DeleteServicioExtraAsync(int servicioTerapeutaId, int servicioExtraId);
        Task<ServicioExtraCatalogoDto> CreateServicioExtraCatalogoAsync(CreateServicioExtraCatalogoDto dto);
        Task<ServicioExtraCatalogoDto> UpdateServicioExtraCatalogoAsync(int id, UpdateServicioExtraCatalogoDto dto);
        Task<bool> DeleteServicioExtraCatalogoAsync(int id);
        Task<ServicioExtraCatalogoDto> GetServicioExtraCatalogoByIdAsync(int id);
    }
}
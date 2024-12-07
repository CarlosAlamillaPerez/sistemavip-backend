using SistemaVIP.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IComisionService
    {
        // Operaciones básicas
        Task<List<ComisionDto>> GetAllAsync();
        Task<ComisionDto> GetByIdAsync(int id);
        Task<List<ComisionDto>> GetByFiltroAsync(FiltroComisionesDto filtro);

        // Cálculos y resúmenes
        Task<ComisionDto> CalcularComisionServicioAsync(int servicioId);
        Task<List<ResumenComisionesDto>> GetResumenPresentadoresAsync();
        Task<ResumenComisionesDto> GetResumenPresentadorAsync(int presentadorId);

        // Gestión de estados
        Task<ComisionDto> RegistrarPagoTerapeutaAsync(int comisionId, RegistroPagoComisionDto dto);
        Task<ComisionDto> ConfirmarPagoAsync(int comisionId, ConfirmacionPagoComisionDto dto);
        Task<ComisionDto> LiquidarPagoAsync(int comisionId, LiquidacionComisionDto dto);

        // Reportes
        Task<List<ComisionDto>> GetComisionesPendientesLiquidacionAsync(int presentadorId);
        Task<List<ComisionDto>> GetComisionesLiquidadasAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
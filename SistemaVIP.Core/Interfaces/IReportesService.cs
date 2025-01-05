using SistemaVIP.Core.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface IReportesService
    {
        // Reportes de Presentadores (mantienen su implementación anterior)
        Task<List<ReportePresentadorDto>> GetReportePresentadoresAsync(ReportesPresentadorFiltroDto filtro);
        Task<ReportePresentadorDetalladoDto> GetReportePresentadorAsync(int presentadorId, DateTime fechaInicio, DateTime fechaFin);

        // Reportes de Terapeutas (actualizados con los nuevos tipos)
        Task<List<ReporteTerapeutaResumenDto>> GetReporteTerapeutasAsync(ReporteTerapeutaFiltroDto filtro);
        Task<ReporteTerapeutaDetalladoDto> GetReporteTerapeutaAsync(int terapeutaId, DateTime fechaInicio, DateTime fechaFin);

        // Reporte de Servicios (se mantiene igual)
        Task<ReporteServiciosDto> GetReporteServiciosAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}

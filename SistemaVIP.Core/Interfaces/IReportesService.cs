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
        Task<List<ReportePresentadorDto>> GetReportePresentadoresAsync(ReportesPresentadorFiltroDto filtro);
        Task<ReportePresentadorDto> GetReportePresentadorAsync(int presentadorId, DateTime fechaInicio, DateTime fechaFin);
        
        //Métodos para terapeutas
        Task<List<ReporteTerapeutaDto>> GetReporteTerapeutasAsync(ReporteTerapeutaFiltroDto filtro);
        Task<ReporteTerapeutaDto> GetReporteTerapeutaAsync(int terapeutaId, DateTime fechaInicio, DateTime fechaFin);

        //Métodos para análisis de servicios
        Task<ReporteServiciosDto> GetReporteServiciosAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}

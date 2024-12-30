using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVIP.Core.DTOs
{
    public class ComisionDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int TerapeutaId { get; set; }
        public int PresentadorId { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoTerapeuta { get; set; }
        public decimal MontoComisionTotal { get; set; }
        public decimal MontoComisionEmpresa { get; set; }
        public decimal MontoComisionPresentador { get; set; }
        public decimal PorcentajeAplicadoEmpresa { get; set; }
        public decimal PorcentajeAplicadoPresentador { get; set; }
        public DateTime FechaCalculo { get; set; }
        public string Estado { get; set; }
        public string? NumeroTransaccion { get; set; }
        public string? ComprobanteUrl { get; set; }
        public DateTime? FechaPagoTerapeuta { get; set; }
        public DateTime? FechaLiquidacionPresentador { get; set; }
        public string? NotasPago { get; set; }

        // Propiedades de navegación simplificadas
        public string NombrePresentador { get; set; }
        public string NombreTerapeuta { get; set; }

        // Datos de tracking
        public string? IdUsuarioConfirmacion { get; set; }
        public string? NombreUsuarioConfirmacion { get; set; }
        public DateTime? FechaConfirmacion { get; set; }

        public string? IdUsuarioLiquidacion { get; set; }
        public string? NombreUsuarioLiquidacion { get; set; }
        public DateTime? FechaLiquidacion { get; set; }
    }

    public class RegistroPagoComisionDto
    {
        public string NumeroTransaccion { get; set; }
        public string ComprobanteUrl { get; set; }
        public string? NotasPago { get; set; }
    }
    public class CambioEstadoComisionDto
    {
        [Required]
        public string Estado { get; set; }
        public string? Notas { get; set; }
    }
    public class ResumenComisionesDto
    {
        public int PresentadorId { get; set; }
        public string NombrePresentador { get; set; }
        public decimal TotalPendientePago { get; set; }
        public decimal TotalPorConfirmar { get; set; }
        public decimal TotalConfirmado { get; set; }
        public int CantidadServiciosPendientes { get; set; }
        public int CantidadServiciosPorConfirmar { get; set; }
        public int CantidadServiciosConfirmados { get; set; }
        public DateTime? UltimaLiquidacion { get; set; }
    }

    public class FiltroComisionesDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? PresentadorId { get; set; }
        public int? TerapeutaId { get; set; }
        public string? Estado { get; set; }
    }
}
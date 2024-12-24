using System;
using NetTopologySuite.Geometries;

namespace SistemaVIP.Core.Models
{
    public class ServiciosTerapeutasModel
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int TerapeutaId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public NetTopologySuite.Geometries.Point? UbicacionInicio { get; set; }
        public NetTopologySuite.Geometries.Point? UbicacionFin { get; set; }
        public string Estado { get; set; }

        // Montos y pagos
        public decimal? MontoTerapeuta { get; set; }
        public decimal? GastosTransporte { get; set; }
        public string? NotasTransporte { get; set; }
        public decimal? MontoEfectivo { get; set; }
        public decimal? MontoTransferencia { get; set; }
        public string? EvidenciaTransporte { get; set; } 
        public DateTime? FechaRegistroGastosTransporte { get; set; }

        public decimal? MontoTotalPagado { get => ComprobantesPago?.Sum(cp => cp.Monto) ?? 0; }
        public bool PagoCompleto { get => MontoTotalPagado >= MontoTerapeuta; }

        public List<CambioEstadoServicioModel> HistorialEstados { get; set; }

        public List<ComprobantePagoModel> ComprobantesPago { get; set; }

        // Links y confirmaciones
        public Guid LinkConfirmacion { get; set; }
        public Guid LinkFinalizacion { get; set; }

        // Comprobantes y evidencias de pago
        public DateTime? FechaComprobantePago { get; set; }
        public string? UrlComprobantePago { get; set; }  // Para múltiples URLs separadas por coma

        // Confirmación del presentador
        public string? IdPresentadorConfirmaPago { get; set; }
        public string? NotasPago { get; set; }

        // Referencias a las entidades relacionadas
        public ServiciosModel Servicio { get; set; }
        public TerapeutaModel Terapeuta { get; set; }
        public ApplicationUserModel? PresentadorConfirmaPago { get; set; }
    }

    public class ComprobantePagoModel
    {
        public int Id { get; set; }
        public int ServicioTerapeutaId { get; set; }
        public string TipoComprobante { get; set; }
        public string OrigenPago { get; set; }  
        public string? NumeroOperacion { get; set; }
        public string UrlComprobante { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; }
        public string? NotasComprobante { get; set; }
        public string IdUsuarioRegistro { get; set; }
        public decimal Monto { get; set; }
        public string? MotivoRechazo { get; set; }  

        // Referencias a las entidades relacionadas
        public ServiciosTerapeutasModel ServicioTerapeuta { get; set; }
        public ApplicationUserModel UsuarioRegistro { get; set; }
    }

    public class CambioEstadoServicioModel
    {
        public int Id { get; set; }
        public int ServicioTerapeutaId { get; set; }
        public string EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; }
        public string? MotivosCambio { get; set; }
        public string IdUsuarioCambio { get; set; }

        // Navegación
        public ServiciosTerapeutasModel ServicioTerapeuta { get; set; }
        public ApplicationUserModel UsuarioCambio { get; set; }
    }
}
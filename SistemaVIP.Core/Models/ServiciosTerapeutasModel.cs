using System;
using NetTopologySuite.Geometries;

namespace SistemaVIP.Core.Models
{
    public class ServiciosTerapeutasModel
    {
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
        public decimal? MontoEfectivo { get; set; }
        public decimal? MontoTransferencia { get; set; }
        public decimal? GastosTransporte { get; set; }
        public string? NotasTransporte { get; set; }

        // Links y confirmaciones
        public Guid LinkConfirmacion { get; set; }
        public Guid LinkFinalizacion { get; set; }

        // Comprobantes y evidencias de pago
        public bool ComprobantePagoTerapeuta { get; set; }
        public DateTime? FechaComprobantePago { get; set; }
        public string? NumeroMovimientoBancario { get; set; }
        public string? TipoMovimiento { get; set; }  // EFECTIVO, TRANSFERENCIA, MIXTO
        public string? UrlComprobantePago { get; set; }  // Para múltiples URLs separadas por coma

        // Confirmación del presentador
        public string? IdPresentadorConfirmaPago { get; set; }
        public string? NotasPago { get; set; }

        // Referencias a las entidades relacionadas
        public ServiciosModel Servicio { get; set; }
        public TerapeutaModel Terapeuta { get; set; }
        public ApplicationUserModel? PresentadorConfirmaPago { get; set; }
    }
}
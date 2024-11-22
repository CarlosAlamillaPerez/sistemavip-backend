using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class ServiciosTerapeutasModel
    {
        public int ServicioId { get; set; }
        public int TerapeutaId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public object? UbicacionInicio { get; set; }  // Tipo GEOGRAPHY en SQL
        public object? UbicacionFin { get; set; }     // Tipo GEOGRAPHY en SQL
        public string Estado { get; set; }
        public decimal? MontoTerapeuta { get; set; }
        public Guid LinkConfirmacion { get; set; }
        public Guid LinkFinalizacion { get; set; }
        public bool ComprobantePagoTerapeuta { get; set; }
        public DateTime? FechaComprobantePago { get; set; }
        public string? NumeroMovimientoBancario { get; set; }
        public string? TipoMovimiento { get; set; }
        public string? UrlComprobanteTransferencia { get; set; }
        public string? IdPresentadorConfirmaPago { get; set; }
        public string? NotasPago { get; set; }

        // Referencias a las entidades relacionadas
        public ServiciosModel Servicio { get; set; }
        public TerapeutaModel Terapeuta { get; set; }
        public ApplicationUserModel? PresentadorConfirmaPago { get; set; }
    }
}
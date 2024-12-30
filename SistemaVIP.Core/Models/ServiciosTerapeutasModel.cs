using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace SistemaVIP.Core.Models
{
    public class ServiciosTerapeutasModel
    {
        [Key]
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int TerapeutaId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public NetTopologySuite.Geometries.Point? UbicacionInicio { get; set; }
        public NetTopologySuite.Geometries.Point? UbicacionFin { get; set; }
        public string Estado { get; set; }

        public virtual ServiciosModel Servicio { get; set; }
        public virtual TerapeutaModel Terapeuta { get; set; }

        // Montos y pagos
        public decimal? MontoTerapeuta { get; set; }

        public List<ComprobantePagoModel> ComprobantesPago { get; set; }

        // Links y confirmaciones
        public Guid LinkConfirmacion { get; set; }
        public Guid LinkFinalizacion { get; set; }
        public virtual ICollection<ServicioExtraModel> ServiciosExtra { get; set; }
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
}
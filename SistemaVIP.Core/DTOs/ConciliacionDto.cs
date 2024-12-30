using System;
using System.Collections.Generic;

namespace SistemaVIP.Core.DTOs.Servicio
{
    public class ConciliacionServicioDto
    {
        public int ServicioId { get; set; }
        public int TerapeutaId { get; set; }
        public DateTime FechaServicio { get; set; }
        public string TipoUbicacion { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoTerapeuta { get; set; }
        public decimal? GastosTransporte { get; set; }
        public List<ComprobanteConciliacionDto> Comprobantes { get; set; }
        public string Estado { get; set; }
        public string ResultadoConciliacion { get; set; }
        public List<string> Discrepancias { get; set; }
        public decimal MontoServiciosExtra { get; set; }
        public List<ServicioExtraDetalleDto> ServiciosExtra { get; set; }
    }

    public class ComprobanteConciliacionDto
    {
        public int Id { get; set; }
        public string TipoComprobante { get; set; }
        public string OrigenPago { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NumeroOperacion { get; set; }
    }

    public class ResultadoConciliacionDto
    {
        public bool Exitoso { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoComprobantes { get; set; }
        public decimal? Diferencia { get; set; }
        public List<string> Observaciones { get; set; }
        public bool RequiereRevision { get; set; }
    }
}
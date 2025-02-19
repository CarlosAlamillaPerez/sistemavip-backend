﻿using SistemaVIP.Core.DTOs.Servicio;
using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVIP.Core.DTOs
{
    public class ComprobantePagoDto
    {
        public int Id { get; set; }
        public int ServicioTerapeutaId { get; set; }
        public string TipoComprobante { get; set; }
        public string OrigenPago { get; set; }  // Nuevo campo
        public string? NumeroOperacion { get; set; }
        public string UrlComprobante { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; }
        public string? NotasComprobante { get; set; }
        public string? MotivoRechazo { get; set; }  // Nuevo campo
        public string NombreUsuarioRegistro { get; set; }
        public decimal Monto { get; set; }
    }

    public class ComprobanteViewModel
    {
        public ServicioDto Servicio { get; set; }
        public List<ComprobantePagoDto> Comprobantes { get; set; }
        public bool TieneComprobantesPagados { get; set; }
    }

    public class CreateComprobantesMultiplesDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un comprobante")]
        public List<CreateComprobantePagoDto> Comprobantes { get; set; }
    }

    public class CreateComprobantePagoDto
    {
        [Required]
        [StringLength(20)]
        public string TipoComprobante { get; set; }

        [Required]
        [StringLength(20)]
        public string OrigenPago { get; set; }

        [StringLength(50)]
        public string? NumeroOperacion { get; set; }

        [StringLength(500)]
        public string? UrlComprobante { get; set; }

        [StringLength(500)]
        public string? NotasComprobante { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Monto { get; set; }
    }

    public class UpdateComprobanteEstadoDto
    {
        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        [StringLength(500)]
        public string? MotivoRechazo { get; set; }  // Nuevo campo

        [StringLength(500)]
        public string? NotasComprobante { get; set; }
    }
}
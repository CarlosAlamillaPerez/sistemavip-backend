using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SistemaVIP.Core.DTOs.Terapeuta;
using SistemaVIP.Core.DTOs.Presentador;

namespace SistemaVIP.Core.DTOs.Servicio
{
    public class ServicioDto
    {
        public int Id { get; set; }
        public int PresentadorId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaServicio { get; set; }
        public string TipoUbicacion { get; set; }  
        public string? Direccion { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal? GastosTransporte { get; set; }  
        public string? NotasTransporte { get; set; }    
        public string Estado { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
        public string? NotasCancelacion { get; set; }
        public string? Notas { get; set; }

        // Propiedades de navegación
        public string NombrePresentador { get; set; }
        public List<ServicioTerapeutaDto> Terapeutas { get; set; }
        public int DuracionHoras { get; set; }
    }

    public class CreateServicioDto
    {
        [Required]
        public int PresentadorId { get; set; }

        [Required]
        public DateTime FechaServicio { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoUbicacion { get; set; }  // Nuevo campo

        [StringLength(500)]
        public string? Direccion { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal MontoTotal { get; set; }

        [Range(0, 10000)]
        public decimal? GastosTransporte { get; set; }  // Nuevo campo

        public string? NotasTransporte { get; set; }    // Nuevo campo

        [Required]
        public List<CreateServicioTerapeutaDto> Terapeutas { get; set; }

        public string? Notas { get; set; }

        [Required]
        [Range(1, 24)] // Máximo 24 horas por servicio
        public int DuracionHoras { get; set; }

    }

    public class ServicioTerapeutaDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int TerapeutaId { get; set; }
        public string NombreTerapeuta { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public string Estado { get; set; }
        public decimal? MontoTerapeuta { get; set; }
        public Guid LinkConfirmacion { get; set; }
        public Guid LinkFinalizacion { get; set; }
        public decimal? MontoEfectivo { get; set; }
        public decimal? MontoTransferencia { get; set; }
        public List<ComprobantePagoDto> ComprobantesPago { get; set; }
    }

    public class CreateServicioTerapeutaDto
    {
        [Required]
        public int TerapeutaId { get; set; }

        [Required]
        [Range(0, 190000)]
        public decimal MontoTerapeuta { get; set; }

        [Range(0, 190000)]
        public decimal? MontoEfectivo { get; set; }

        [Range(0, 190000)]
        public decimal? MontoTransferencia { get; set; }
    }

    public class UpdateServicioDto
    {
        public DateTime? FechaServicio { get; set; }
        public string? Direccion { get; set; }  // Cambiado a opcional
        public decimal? MontoTotal { get; set; }
        public string? Notas { get; set; }
        public int? DuracionHoras { get; set; }  // Agregado como opcional
    }

    public class ConfirmacionServicioDto
    {
        [Required]
        public Guid LinkConfirmacion { get; set; }

        [Required]
        public double Latitud { get; set; }

        [Required]
        public double Longitud { get; set; }
    }

    public class FinalizacionServicioDto
    {
        [Required]
        public Guid LinkFinalizacion { get; set; }

        [Required]
        public double Latitud { get; set; }

        [Required]
        public double Longitud { get; set; }
    }

    public class CancelacionServicioDto
    {
        [Required]
        [StringLength(255)]
        public string MotivoCancelacion { get; set; }

        public string NotasCancelacion { get; set; }
    }

    public class CreateServicioExtraDto
    {
        public List<ServicioExtraItemDto> ServiciosExtra { get; set; }
    }

    public class ServicioExtraItemDto
    {
        public int ServicioExtraCatalogoId { get; set; }
        public decimal Monto { get; set; }
        public string? Notas { get; set; }
    }
    public class ServicioExtraDetalleDto
    {
        public int Id { get; set; }
        public int ServicioExtraCatalogoId { get; set; }
        public string NombreServicio { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Notas { get; set; }
    }

    public class UpdateServicioExtraDto
    {
        public decimal Monto { get; set; }
        public string? Notas { get; set; }
    }
}
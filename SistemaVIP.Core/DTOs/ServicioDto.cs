﻿using System;
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
        public string TipoServicio { get; set; }
        public string Direccion { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string MotivoCancelacion { get; set; }
        public string NotasCancelacion { get; set; }
        public string Notas { get; set; }

        // Propiedades de navegación
        public string NombrePresentador { get; set; }
        public List<ServicioTerapeutaDto> Terapeutas { get; set; }
    }

    public class CreateServicioDto
    {
        [Required]
        public int PresentadorId { get; set; }

        [Required]
        public DateTime FechaServicio { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoServicio { get; set; }

        [StringLength(500)]
        public string Direccion { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal MontoTotal { get; set; }

        [Required]
        public List<CreateServicioTerapeutaDto> Terapeutas { get; set; }

        public string Notas { get; set; }
    }

    public class ServicioTerapeutaDto
    {
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
        public bool ComprobantePagoTerapeuta { get; set; }
    }

    public class CreateServicioTerapeutaDto
    {
        [Required]
        public int TerapeutaId { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal MontoTerapeuta { get; set; }
    }

    public class UpdateServicioDto
    {
        public DateTime? FechaServicio { get; set; }
        public string Direccion { get; set; }
        public decimal? MontoTotal { get; set; }
        public string Notas { get; set; }
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
}
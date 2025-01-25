using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVIP.Core.DTOs.TerapeutaPresentador
{
    // TerapeutaPresentadorDto.cs
    public class TerapeutaPresentadorDto
    {
        public int TerapeutaId { get; set; }
        public int PresentadorId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string Estado { get; set; }

        // Información del Terapeuta
        public string NombreTerapeuta { get; set; }
        public string ApellidoTerapeuta { get; set; }
        public string TelefonoTerapeuta { get; set; }
        public string EmailTerapeuta { get; set; }
        public string EstadoTerapeuta { get; set; }

        // Información del Presentador
        public string NombrePresentador { get; set; }
        public string ApellidoPresentador { get; set; }
        public string EstadoPresentador { get; set; }
    }

    // Para la respuesta agrupada podemos crear un DTO específico:
    public class AsignacionesPresentadorDto
    {
        public int PresentadorId { get; set; }
        public string NombreCompleto { get; set; }
        public string Estado { get; set; }
        public int CantidadTerapeutas { get; set; }
        public DateTime? UltimaAsignacion { get; set; }
        public List<TerapeutaAsignadaDto> TerapeutasAsignadas { get; set; }
    }

    public class TerapeutaAsignadaDto
    {
        public int TerapeutaId { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public DateTime FechaAsignacion { get; set; }
    }

    public class AsignarTerapeutaPresentadorDto
    {
        [Required]
        public int TerapeutaId { get; set; }

        [Required]
        public int PresentadorId { get; set; }
    }

    public class TerapeutasPorPresentadorDto
    {
        public int TerapeutaId { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public DateTime FechaAsignacion { get; set; }
    }
}
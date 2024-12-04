using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVIP.Core.DTOs.TerapeutaPresentador
{
    public class TerapeutaPresentadorDto
    {
        public int TerapeutaId { get; set; }
        public int PresentadorId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string Estado { get; set; }

        // Propiedades de navegación para mostrar información básica
        public string NombreTerapeuta { get; set; }
        public string ApellidoTerapeuta { get; set; }
        public string NombrePresentador { get; set; }
        public string ApellidoPresentador { get; set; }
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
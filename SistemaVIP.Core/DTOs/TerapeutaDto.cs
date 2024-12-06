using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVIP.Core.DTOs.Terapeuta
{
    public class TerapeutaDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCambioEstado { get; set; }
        public string? MotivoEstado { get; set; }
        public string Estatura { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string? FotoUrl { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string? Notas { get; set; }
        public decimal TarifaBase { get; set; }
        public decimal TarifaExtra { get; set; }
    }

    public class CreateTerapeutaDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(20)]
        public string Estatura { get; set; }

        [Required]
        [StringLength(20)]
        public string DocumentoIdentidad { get; set; }

        public string? FotoUrl { get; set; }

        public string? Notas { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal TarifaBase { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal TarifaExtra { get; set; }
    }

    public class UpdateTerapeutaDto
    {
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(100)]
        public string? Apellido { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Estatura { get; set; }

        public string? FotoUrl { get; set; }

        public string? Notas { get; set; }
    }
}
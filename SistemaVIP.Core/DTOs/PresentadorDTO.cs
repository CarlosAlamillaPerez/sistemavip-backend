﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVIP.Core.DTOs.Presentador
{
    public class PresentadorDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public decimal PorcentajeComision { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCambioEstado { get; set; }
        public string? MotivoEstado { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string? FotoUrl { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string? Notas { get; set; }
    }

    public class CreatePresentadorDto
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
        [StringLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial")]
        public string Password { get; set; }

        [Range(0, 100)]
        public decimal PorcentajeComision { get; set; }

        [Required]
        [StringLength(20)]
        public string DocumentoIdentidad { get; set; }

        public string? FotoUrl { get; set; }

        public string? Notas { get; set; }
    }

    public class UpdatePresentadorDto
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

        public string? FotoUrl { get; set; }

        public string? Notas { get; set; }
    }
}
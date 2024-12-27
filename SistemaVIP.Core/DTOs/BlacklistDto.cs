using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaVIP.Core.DTOs
{
    // DTO para respuestas y consultas
    public class BlacklistDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string IdUsuarioRegistro { get; set; }
        public string NombreUsuarioRegistro { get; set; }
        public string Estado { get; set; }
        public string Notas { get; set; }
    }

    // DTO para crear nuevos registros
    public class CreateBlacklistDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "El motivo es requerido")]
        [StringLength(500)]
        public string Motivo { get; set; }

        [StringLength(1000)]
        public string Notas { get; set; }
    }

    // DTO para actualizar registros existentes
    public class UpdateBlacklistDto
    {
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(500)]
        public string Motivo { get; set; }

        [StringLength(1000)]
        public string Notas { get; set; }
    }

    // DTO para búsquedas y filtros
    public class BlacklistFiltroDto
    {
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; }
    }

    // DTO para verificación rápida
    public class BlacklistVerificacionDto
    {
        public bool EstaEnBlacklist { get; set; }
        public string Motivo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
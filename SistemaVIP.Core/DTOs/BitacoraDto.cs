using System;

namespace SistemaVIP.Core.DTOs
{
    public class BitacoraDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Accion { get; set; }
        public string Tabla { get; set; }
        public string IdRegistro { get; set; }
        public string ValoresAnteriores { get; set; }
        public string ValoresNuevos { get; set; }
    }

    public class BitacoraFiltroDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Tabla { get; set; }
        public string IdUsuario { get; set; }
        public string Accion { get; set; }
    }
}
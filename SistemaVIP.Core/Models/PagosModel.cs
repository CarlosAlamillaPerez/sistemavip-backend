using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Models
{
    public class PagosModel
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string? TipoTransferencia { get; set; }
        public string? NumeroOperacion { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Estado { get; set; }
        public string IdUsuarioRegistro { get; set; }
        public DateTime? FechaValidacion { get; set; }
        public string? IdUsuarioValidacion { get; set; }
        public string? NotasValidacion { get; set; }

        // Referencias a las entidades relacionadas
        public ServiciosModel Servicio { get; set; }
        public ApplicationUserModel UsuarioRegistro { get; set; }
        public ApplicationUserModel? UsuarioValidacion { get; set; }
    }
}
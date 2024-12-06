using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Enums
{
    public static class EstadosEnum
    {
        public static class General
        {
            public const string ACTIVO = "ACTIVO";
            public const string INACTIVO = "INACTIVO";
            public const string SUSPENDIDO = "SUSPENDIDO";
        }

        public static class Servicio
        {
            public const string PENDIENTE = "PENDIENTE";
            public const string EN_PROCESO = "EN_PROCESO";
            public const string FINALIZADO = "FINALIZADO";
            public const string CANCELADO = "CANCELADO";
        }

        public static class TipoEntidad
        {
            public const string GENERAL = "GENERAL";
            public const string SERVICIO = "SERVICIO";
        }

        public static readonly string[] EstadosGenerales = new[]
        {
            General.ACTIVO,
            General.INACTIVO,
            General.SUSPENDIDO
        };

        public static readonly string[] EstadosServicio = new[]
        {
            Servicio.PENDIENTE,
            Servicio.EN_PROCESO,
            Servicio.FINALIZADO,
            Servicio.CANCELADO
        };
    }
}
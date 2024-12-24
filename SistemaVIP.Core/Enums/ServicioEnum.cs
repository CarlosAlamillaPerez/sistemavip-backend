using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Enums
{
    public static class ServicioEnum
    {
        public static class TipoUbicacion
        {
            public const string DOMICILIO = "DOMICILIO";
            public const string CONSULTORIO = "CONSULTORIO";
        }

        public static readonly string[] TiposUbicacion = new[]
        {
            TipoUbicacion.DOMICILIO,
            TipoUbicacion.CONSULTORIO
        };

        // Podemos agregar validaciones para ubicación
        public static class ValidacionUbicacion
        {
            public const double RADIO_MAXIMO_KM = 0.5; // Radio máximo en kilómetros para validar ubicación
            public const double RADIO_MAXIMO_DOMICILIO_KM = 50.0; // Radio máximo para servicios a domicilio
        }
    }
}

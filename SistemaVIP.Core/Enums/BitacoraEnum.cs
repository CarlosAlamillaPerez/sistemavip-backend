namespace SistemaVIP.Core.Enums
{
    public static class BitacoraEnum
    {
        public static class TipoAccion
        {
            public const string CREACION = "CREACION";
            public const string MODIFICACION = "MODIFICACION";
            public const string CAMBIO_ESTADO = "CAMBIO_ESTADO";
            public const string VALIDACION = "VALIDACION";
            public const string REGISTRO_PAGO = "REGISTRO_PAGO";
            public const string CONFIRMACION_PAGO = "CONFIRMACION_PAGO";
            public const string RECHAZO_PAGO = "RECHAZO_PAGO";
            public const string VALIDACION_UBICACION = "VALIDACION_UBICACION";
            public const string CONCILIACION = "CONCILIACION";
            public const string GENERACION_COMISION = "GENERACION_COMISION";
        }

        public static class TablaMonitoreo
        {
            public const string SERVICIOS = "Servicios";
            public const string SERVICIOS_TERAPEUTAS = "ServiciosTerapeutas";
            public const string COMPROBANTES_PAGO = "ComprobantesPago";
            public const string COMISIONES = "Comisiones";
            public const string TERAPEUTAS = "Terapeutas";
            public const string PRESENTADORES = "Presentadores";
            public const string BLACKLIST = "Blacklist";  // Agregamos esta línea
        }

        public static readonly string[] TiposAccion = new[]
        {
            TipoAccion.CREACION,
            TipoAccion.MODIFICACION,
            TipoAccion.CAMBIO_ESTADO,
            TipoAccion.VALIDACION,
            TipoAccion.REGISTRO_PAGO,
            TipoAccion.CONFIRMACION_PAGO,
            TipoAccion.RECHAZO_PAGO,
            TipoAccion.VALIDACION_UBICACION,
            TipoAccion.CONCILIACION,
            TipoAccion.GENERACION_COMISION
        };

        public static readonly string[] TablasMonitoreo = new[]
        {
            TablaMonitoreo.SERVICIOS,
            TablaMonitoreo.SERVICIOS_TERAPEUTAS,
            TablaMonitoreo.COMPROBANTES_PAGO,
            TablaMonitoreo.COMISIONES,
            TablaMonitoreo.TERAPEUTAS,
            TablaMonitoreo.PRESENTADORES,
            TablaMonitoreo.BLACKLIST  // Agregamos esta línea también
        };
    }
}
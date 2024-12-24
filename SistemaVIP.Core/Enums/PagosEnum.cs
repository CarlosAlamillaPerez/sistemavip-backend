namespace SistemaVIP.Core.Enums
{
    public static class PagosEnum
    {
        public static class TipoComprobante
        {
            public const string EFECTIVO = "EFECTIVO";
            public const string TRANSFERENCIA = "TRANSFERENCIA";
            public const string MIXTO = "MIXTO";
        }

        public static class OrigenPago
        {
            public const string PAGO_CLIENTE = "PAGO_CLIENTE";
            public const string COMISION_TERAPEUTA = "COMISION_TERAPEUTA";
        }

        public static class EstadoComprobante
        {
            public const string PENDIENTE = "PENDIENTE";
            public const string POR_CONFIRMAR = "POR_CONFIRMAR";
            public const string PAGADO = "PAGADO";
            public const string RECHAZADO = "RECHAZADO";
        }

        public static readonly string[] TiposComprobante = new[]
        {
            TipoComprobante.EFECTIVO,
            TipoComprobante.TRANSFERENCIA,
            TipoComprobante.MIXTO
        };

        public static readonly string[] OrigenesComprobante = new[]
        {
            OrigenPago.PAGO_CLIENTE,
            OrigenPago.COMISION_TERAPEUTA
        };

        public static readonly string[] EstadosComprobante = new[]
        {
            EstadoComprobante.PENDIENTE,
            EstadoComprobante.POR_CONFIRMAR,
            EstadoComprobante.PAGADO,
            EstadoComprobante.RECHAZADO
        };
    }
}
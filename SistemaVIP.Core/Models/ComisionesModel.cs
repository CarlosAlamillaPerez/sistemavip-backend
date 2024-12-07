using SistemaVIP.Core.Models;

public class ComisionesModel
{
    public int Id { get; set; }
    public int ServicioId { get; set; }
    public int TerapeutaId { get; set; }
    public int PresentadorId { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal MontoTerapeuta { get; set; }
    public decimal MontoComisionTotal { get; set; }
    public decimal MontoComisionEmpresa { get; set; }
    public decimal MontoComisionPresentador { get; set; }
    public decimal PorcentajeAplicadoEmpresa { get; set; }
    public decimal PorcentajeAplicadoPresentador { get; set; }
    public DateTime FechaCalculo { get; set; }
    public string Estado { get; set; }

    // Nuevos campos para el comprobante de pago
    public string? NumeroTransaccion { get; set; }
    public string? ComprobanteUrl { get; set; }
    public DateTime? FechaPagoTerapeuta { get; set; }
    public DateTime? FechaLiquidacionPresentador { get; set; }
    public string? NotasPago { get; set; }

    // Campos adicionales para tracking
    public string? IdUsuarioConfirmacion { get; set; }
    public DateTime? FechaConfirmacion { get; set; }
    public ApplicationUserModel? UsuarioConfirmacion { get; set; }

    public string? IdUsuarioLiquidacion { get; set; }
    public DateTime? FechaLiquidacion { get; set; }
    public ApplicationUserModel? UsuarioLiquidacion { get; set; }

    // Referencias a las entidades relacionadas
    public ServiciosModel Servicio { get; set; }
    public TerapeutaModel Terapeuta { get; set; }
    public PresentadorModel Presentador { get; set; }
}
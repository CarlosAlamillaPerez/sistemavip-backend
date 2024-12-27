using SistemaVIP.Core.Models;

public class CancelacionesPresentadorModel
{
    public int PresentadorId { get; set; }
    public string NombrePresentador { get; set; }
    public int CantidadCancelaciones { get; set; }
    public DateTime SemanaInicio { get; set; }
    public DateTime SemanaFin { get; set; }

    // Navegación
    public PresentadorModel Presentador { get; set; }
}
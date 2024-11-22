using SistemaVIP.Core.DTOs;
using SistemaVIP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Infrastructure.Services
{
    public class ComisionService : IComisionService
    {
        private readonly decimal _porcentajeEmpresa = 0.70m;
        private readonly decimal _porcentajePresentador = 0.30m;

        public ComisionResult CalcularComisiones(decimal montoTotal, decimal montoTerapeuta)
        {
            // El monto base para comisiones es lo que queda después de la comisión del terapeuta
            decimal montoBaseComisiones = montoTotal - montoTerapeuta;

            // Aplicamos los porcentajes configurados (70/30)
            decimal comisionEmpresa = montoBaseComisiones * _porcentajeEmpresa;
            decimal comisionPresentador = montoBaseComisiones * _porcentajePresentador;

            return new ComisionResult
            {
                MontoTotal = montoTotal,
                MontoTerapeuta = montoTerapeuta,
                MontoComisionTotal = montoBaseComisiones,
                MontoComisionEmpresa = comisionEmpresa,
                MontoComisionPresentador = comisionPresentador,
                PorcentajeAplicadoEmpresa = _porcentajeEmpresa * 100,
                PorcentajeAplicadoPresentador = _porcentajePresentador * 100
            };
        }

        public async Task<ComisionResult> CalcularComisionesAsync(decimal montoTotal, decimal montoTerapeuta)
        {
            // Para futuras implementaciones que requieran async
            return await Task.FromResult(CalcularComisiones(montoTotal, montoTerapeuta));
        }
    }
}
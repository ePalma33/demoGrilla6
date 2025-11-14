using demoGrilla6.Data;
using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace demoGrilla6.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PedidoCompraService _pedidoService;
        private readonly PurchLineRepository _purchLineRepository;

        public IndexModel(PedidoCompraService pedidoService, PurchLineRepository purchLineRepository)
        {
            _pedidoService = pedidoService;
            _purchLineRepository = purchLineRepository;
        }

        /// <summary>
        /// Handler para devolver las órdenes de compra
        /// </summary>
        public async Task<JsonResult> OnGetPedidoComprasAsync()
        {
            var pedidos = await _pedidoService.GetPedidoComprasAsync("");
            return new JsonResult(new { data = pedidos });
        }

        /// <summary>
        /// Handler para devolver las líneas de la orden en formato JSON
        /// </summary>
        /// <param name="purchId">ID de la orden de compra</param>
        /// <returns>JSON con las líneas</returns>
        public async Task<JsonResult> OnGetPurchLinesAsync(string purchId)
        {
            if (string.IsNullOrWhiteSpace(purchId))
            {
                return new JsonResult(new { data = Enumerable.Empty<PurchLine>() });
            }

            var lineas = await _purchLineRepository.GetByPurchIdAsync(purchId);
            return new JsonResult(new { data = lineas });
        }
    }
}
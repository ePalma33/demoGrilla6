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

        public IndexModel(PedidoCompraService pedidoService)
        {
            _pedidoService = pedidoService;
        }


        public async Task<JsonResult> OnGetPedidoComprasAsync()
        {
            var pedidos = await _pedidoService.GetPedidoComprasAsync("");
            return new JsonResult(new { data = pedidos });
        }

    }


    public class PurchLineModel : PageModel
    {
        private readonly PurchLineRepository _purchLineRepository;

        public PurchLineModel(PurchLineRepository purchLineRepository)
        {
            _purchLineRepository = purchLineRepository;
        }

        public void OnGet()
        {
            // Página vacía, solo sirve para el handler
        }

        /// <summary>
        /// Handler para devolver las líneas de la orden en formato JSON
        /// </summary>
        /// <param name="purchId">ID de la orden de compra</param>
        /// <returns>JSON con las líneas</returns>
        public async Task<JsonResult> OnGetLinesAsync(string purchId)
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
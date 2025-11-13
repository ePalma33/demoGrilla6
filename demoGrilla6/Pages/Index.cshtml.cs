using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using demoGrilla6.Services;

namespace demoGrilla6.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PedidoCompraService _pedidoService;

        public IndexModel(PedidoCompraService pedidoService)
        {
            _pedidoService = pedidoService;
        }


        public async Task<JsonResult> OnGetAutosAsync()
        {
            var pedidos = await _pedidoService.GetPedidoComprasAsync("");
            return new JsonResult(new { data = pedidos });
        }

    }
}
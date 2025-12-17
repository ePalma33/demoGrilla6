using demoGrilla6.Data;
using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace demoGrilla6.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OrdenCompraDetService _pedidoService;
        private readonly OrdenCompraDetRepository _purchLineRepository;
        private readonly PagoFacturaRepository _pagoFacturaRepository;
        private readonly RecepcionCabRepository _recepcionRepository;

        public async Task<IActionResult> OnGetAsync()
        {
            // Solo carga si está vacío o no existe
            if (TempData.Peek("TotalOcPendientes") == null)
            {
                await CargarTempDataAsync();
                // Si quieres que sobreviva al siguiente request, puedes llamar:
                // TempData.Keep(); // o Keep("MontoPorFacturar") para un key concreto
            }

            return Page(); // Renderiza la vista Index y el _Layout en el mismo request
        }



        public IndexModel(OrdenCompraDetService pedidoService, OrdenCompraDetRepository purchLineRepository, PagoFacturaRepository pagoFacturaRepository, RecepcionCabRepository recepcionRepository)
        {
            _pedidoService = pedidoService;
            _purchLineRepository = purchLineRepository;
            _pagoFacturaRepository = pagoFacturaRepository;
            _recepcionRepository = recepcionRepository;
        }

        /// <summary>
        /// Handler para devolver las órdenes de compra
        /// </summary>
        public async Task<JsonResult> OnGetPedidoComprasAsync()
        {
            var proveedor = User.FindFirst("Proveedor")?.Value;
            var pedidos = await _pedidoService.GetPedidoComprasAsync(proveedor);
            
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
                return new JsonResult(new { data = Enumerable.Empty<OrdenCompraDet>() });
            }

            var lineas = await _purchLineRepository.GetByPurchIdAsync(purchId);
            return new JsonResult(new { data = lineas });
        }

        public async Task CargarTempDataAsync()
        {
            var proveedor = User.FindFirst("Proveedor")?.Value; // Recupera de la sesión
            var pedidos = await _pedidoService.GetPedidoComprasAsync(proveedor);
            var ultimoPago = await _pagoFacturaRepository.GetUltimoPagoAsync(proveedor);
            var recepcionNofacturada = await _recepcionRepository.GetNumeroNoFacturadoAsync(proveedor);

            TempData["TotalOcPendientes"] = pedidos.Count(p => p.PurchStatusText == "Pedido abierto");
            TempData["TotalOcRecibido"] = pedidos.Count(p => p.PurchStatusText == "Recibido");
            TempData["TotalOcFacturado"] = pedidos.Count(p => p.PurchStatusText == "Facturado");
            TempData["TotalOcCancelado"] = pedidos.Count(p => p.PurchStatusText == "Cancelado");

            TempData["TotalOcEDborrador"] = pedidos.Count(p => p.DocumentStateText == "Borrador");
            TempData["TotalOcEDenRevision"] = pedidos.Count(p => p.DocumentStateText == "En revision");
            TempData["TotalOcEDrechazado"] = pedidos.Count(p => p.DocumentStateText == "Rechazado");
            TempData["TotalOcEDaprobado"] = pedidos.Count(p => p.DocumentStateText == "Aprobado");
            TempData["TotalOcEDrevisionExterna"] = pedidos.Count(p => p.DocumentStateText == "En revisión externa");
            TempData["TotalOcEDfinalizada"] = pedidos.Count(p => p.DocumentStateText == "Finalizada");
            TempData["TotalOcEDconfirmado"] = pedidos.Count(p => p.DocumentStateText == "Confirmado");
            TempData["TotalOcEDdesconocido"] = pedidos.Count(p => p.DocumentStateText == "Desconocido");

            if (pedidos.Count() > 0)
            {
                TempData["NombreProveedor"] = pedidos.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.PurchName)).PurchName;
            }
            else
            {
                TempData["NombreProveedor"] = "Sin Nombre";
            }

            TempData["NumeroRecepcionNoFacturada"] = recepcionNofacturada.FirstOrDefault().Numero;

            var pago = ultimoPago?.FirstOrDefault();

            // Resuelve el nullable con un default double:
            double montoDouble = pago?.MontoPago ?? 0.0;
            DateTime fechaUltimoPago = pago?.FechaPago ?? new DateTime();

            TempData["MontoUltimoPago"] = "$" + montoDouble.ToString("N0", CultureInfo.GetCultureInfo("es-CL"));
            TempData["FechaUltimoPago"] = fechaUltimoPago;


            TempData["NumeroMayorED"] = pedidos
                .GroupBy(p => p.DocumentStateText)   
                .Select(g => g.Count())
                .DefaultIfEmpty(0)
                .Max();


            var montoPorFacturar = pedidos
                .Where(p => p.DocumentStateText == "Confirmado" &&
                            p.PurchStatusText != "Facturado" &&
                            p.PurchStatusText != "Cancelado")
                .Sum(p => p.TotalAmount);

            // Guarda en TempData para la próxima petición (si la vista lo necesita)
            TempData["MontoPorFacturar"] = ((long)montoPorFacturar).ToString(CultureInfo.InvariantCulture);

        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using demoGrilla6.Services;
using demoGrilla6.Models;

namespace demoGrilla6.Pages
{
    public class FacturaModel : PageModel
    {
        private readonly FacturaService _facturaService;
        private readonly PagoFacturaService _pagoFacturaService;
        public string PurchId { get; set; }


        // Constructor con ambos servicios
        public FacturaModel(FacturaService facturaService, PagoFacturaService pagoFacturaService)
        {
            _facturaService = facturaService;
            _pagoFacturaService = pagoFacturaService;
        }


        /// <summary>
        /// Captura el parámetro en el GET inicial
        /// </summary>
        public void OnGet(string purchId)
        {
            PurchId = purchId;
        }



        public async Task<JsonResult> OnGetFacturaAsync(string purchId)
        {
            try
            {
                IEnumerable<Factura> facturas;
                var proveedor = User.FindFirst("Proveedor")?.Value; // Recupera de la sesión

                if (string.IsNullOrWhiteSpace(purchId))
                {
                    //  Si no hay parámetro, traer todos
                    facturas = await _facturaService.GetInvoicesAsync(proveedor);
                }
                else
                {
                    //  Si hay parámetro, filtrar
                    facturas = await _facturaService.GetInvoicesByPurchIdAsync(purchId);
                }

                return new JsonResult(new { data = facturas });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al obtener Packing Slips", details = ex.Message });
            }
        }

        public async Task<JsonResult> OnGetPagoFacturaAsync(string idFactura, string proveedor, string voucher)
        {
            try
            {
                IEnumerable<PagoFactura> pagoFacturas;

                pagoFacturas = await _pagoFacturaService.GetPagoFacturaAsync(idFactura, proveedor, voucher);

                return new JsonResult(new { data = pagoFacturas });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al obtener pago factura", details = ex.Message });
            }
        }

        public async Task<JsonResult> OnGetTotalPendienteAsync(string idFactura, string proveedor)
        {
            try
            {
                IEnumerable<PagoFacturaTotalPendiente> totaPendiente;

                totaPendiente = await _pagoFacturaService.GetTotalPendienteAsync(idFactura, proveedor);

                return new JsonResult(new { data = totaPendiente });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al obtener pago factura", details = ex.Message });
            }
        }


        public async Task<JsonResult> OnGetUltimoPagoAsync(string username)
        {
            try
            {
                IEnumerable<UltimoPago> ultimosPagos;

                ultimosPagos = await _pagoFacturaService.GetUltimoPagoAsync(username);

                return new JsonResult(new { data = ultimosPagos });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al obtener pago factura", details = ex.Message });
            }
        }
    }
}


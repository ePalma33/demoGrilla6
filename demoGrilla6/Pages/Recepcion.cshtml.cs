using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace demoGrilla6.Pages
{
    public class RecepcionModel : PageModel
    {
        private readonly RecepcionCabService _jourService;
        private readonly RecepcionDetService _transService;

        public string PurchId { get; set; }

        public RecepcionModel(RecepcionCabService jourService, RecepcionDetService transService)
        {
            _jourService = jourService;
            _transService = transService;
        }

        /// <summary>
        /// Captura el parámetro en el GET inicial
        /// </summary>
        public void OnGet(string purchId)
        {
            PurchId = purchId;
        }

        /// <summary>
        /// Handler para devolver los Packing Slips filtrados por PurchId o todos
        /// </summary>
        public async Task<JsonResult> OnGetPackingSlipsAsync(string purchId)
        {
            try
            {
                IEnumerable<RecepcionCab> slips;
                var proveedor = User.FindFirst("Proveedor")?.Value; // Recupera de la sesión

                if (string.IsNullOrWhiteSpace(purchId))
                {
                    //  Si no hay parámetro, traer todos
                    slips = await _jourService.GetPackingSlipsAsync(proveedor);
                }
                else
                {
                    //  Si hay parámetro, filtrar
                    slips = await _jourService.GetPackingSlipByPurchIdAsync(purchId);
                }

                return new JsonResult(new { data = slips });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al obtener Packing Slips", details = ex.Message });
            }
        }

        /// <summary>
        /// Handler para devolver las líneas del Packing Slip (detalle)
        /// </summary>
        public async Task<JsonResult> OnGetPackingSlipLinesAsync(string packingSlipId)
        {
            if (string.IsNullOrWhiteSpace(packingSlipId))
                return new JsonResult(new { data = Enumerable.Empty<object>(), message = "ID inválido" });

            try
            {
                var lines = await _transService.GetLinesByPackingSlipIdAsync(packingSlipId);
                return new JsonResult(new { data = lines });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al obtener líneas", details = ex.Message });
            }
        }

        public async Task<JsonResult> OnGetNumeroNoFacturadoAsync()
        {
            try
            {
                IEnumerable<RecepcionNoFacturada> noFacturadas;
                var username = HttpContext.Session.GetString("Username"); // Recupera de la sesión

                noFacturadas = await _jourService.GetNumeroNoFacturadoAsync(username);

                return new JsonResult(new { data = noFacturadas });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Error al obtener Packing Slips", details = ex.Message });
            }
        }


    }
}
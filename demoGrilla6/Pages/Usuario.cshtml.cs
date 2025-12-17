using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace demoGrilla6.Pages
{
    public class UsuarioModel : PageModel
    {
        private readonly UsuarioService _usuario;

        public UsuarioModel(UsuarioService usuario)
        {
            _usuario = usuario;  
        }


        public void OnGet()
        {
            // Si necesitas precargar algo para la vista, hazlo aquí.
            // No devuelvas JsonResult en este handler.
        }

        public async Task<JsonResult> OnGetDataAsync()
        {
            var rutProveedor = User.FindFirst("Proveedor")?.Value ?? string.Empty;
            var usuarios = await _usuario.GetAllAsync(rutProveedor);
            return new JsonResult(new { data = usuarios });
        }

        public async Task<JsonResult> OnGetActivarAsync(int idUsuario)
        {
            var usuarios = await _usuario.OnGetActivarAsync( idUsuario, true);
            return new JsonResult(new { data = usuarios });
        }


    }
}

using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace demoGrilla6.Pages
{
    [IgnoreAntiforgeryToken] // <-- a nivel de clase
    public class UsuarioModel : PageModel
    {
        private readonly UsuarioService _usuario;

        public UsuarioModel(UsuarioService usuario)
        {
            _usuario = usuario;
        }

        public void OnGet()
        {
            // Precarga si es necesario.
        }

        public async Task<JsonResult> OnGetDataAsync()
        {
            var rutProveedor = User.FindFirst("Proveedor")?.Value ?? string.Empty;
            var usuarios = await _usuario.GetAllAsync(rutProveedor);
            return new JsonResult(new { data = usuarios });
        }

        public async Task<JsonResult> OnGetActivarAsync(int idUsuario)
        {
            var actualizado = await _usuario.OnGetActivarAsync(idUsuario, true);
            if (actualizado is null)
                return new JsonResult(new { ok = false, message = "Usuario no encontrado" }) { StatusCode = 404 };

            return new JsonResult(new { ok = true, data = actualizado });
        }



        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> OnPostGuardarAsync(int id, string userName, string nombre, string apellido, string email, bool isActive, bool esAdmin)
        {
            try
            {
                var rutProveedor = User.FindFirst("Proveedor")?.Value ?? string.Empty;

                var entidad = new User
                {
                    Id = id,
                    UserName = userName,
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    IsActive = isActive,
                    EsAdmin = esAdmin,
                    Proveedor = rutProveedor,
                    CreatedAt = DateTime.Now // si es nuevo
                };

                var resultado  = await _usuario.OnPostGuardarAsync(entidad);

                return new JsonResult(new { ok = true });        //, data = resultado
            }
            catch (Exception ex)
            {
                return new JsonResult(new { ok = false, message = "Error al guardar", detail = ex.Message }) { StatusCode = 500 };
            }
        }

    }
}
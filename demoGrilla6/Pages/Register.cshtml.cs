using demoGrilla6.Data;
using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace demoGrilla6.Pages
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IUserRepository _users;
        private readonly IPasswordService _passwords;

        public RegisterModel(IUserRepository users, IPasswordService passwords)
        {
            _users = users;
            _passwords = passwords;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new RegisterInput(); //  no-nullable

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(Input.Username) ||
                string.IsNullOrWhiteSpace(Input.Email) ||
                string.IsNullOrWhiteSpace(Input.Proveedor) ||
                string.IsNullOrWhiteSpace(Input.Password) ||
                string.IsNullOrWhiteSpace(Input.ConfirmPassword) ||
                string.IsNullOrWhiteSpace(Input.Nombre) ||
                string.IsNullOrWhiteSpace(Input.Apellido))
            {
                ErrorMessage = "Completa todos los campos.";
                return Page();
            }

            if (!string.Equals(Input.Password, Input.ConfirmPassword))
            {
                ErrorMessage = "La confirmación de la contraseña no coincide.";
                return Page();
            }

            if (!IsValidEmail(Input.Email))
            {
                ErrorMessage = "Email inválido.";
                return Page();
            }

            // ¿Usuario ya existe?
            var existing = await _users.GetByUserNameAsync(Input.Username);
            if (existing is not null)
            {
                ErrorMessage = "El usuario ya existe.";
                return Page();
            }

            // Hash con tu servicio (BCrypt)
            var hash = _passwords.Hash(Input.Password);

            // Crear entidad según tu clase User
            var newUser = new User
            {
                UserName = Input.Username.Trim(),
                Email = Input.Email.Trim(),
                PasswordHash = hash,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                Proveedor = Input.Proveedor.Trim(),
                Nombre = Input.Nombre,
                Apellido = Input.Apellido
            };

            var ok = await _users.CreateAsync(newUser);
            if (ok == 0)
            {
                ErrorMessage = "No fue posible crear el usuario.";
                return Page();
            }

            // Redirige al Login
            return RedirectToPage("/Login");
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }
    }

    // Input model para el formulario
    public class RegisterInput
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Proveedor { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public string ConfirmPassword { get; set; } = string.Empty;
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required] public string Apellido { get; set; } = string.Empty;
    }


}

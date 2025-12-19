using Dapper;
using demoGrilla6.Configuration;
using demoGrilla6.Data;
using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Text.Encodings.Web;

namespace demoGrilla6.Pages
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IUserRepository _users;
        private readonly IPasswordService _passwords;


        [TempData] public bool ShowSavedModal { get; set; }
        [TempData] public string? SavedModalText { get; set; }
        //[TempData] public bool ShowErrorToast { get; set; }
        //[TempData] public string? ErrorToastText { get; set; }


        public RegisterModel(IUserRepository users, IPasswordService passwords)
        {
            _users = users;
            _passwords = passwords;

        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new RegisterInput(); //  no-nullable

        //public string? ErrorMessage { get; set; }
        //public string? SuccessMessage { get; set; }

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
                TempData["ShowErrorToast"] = true;
                TempData["ErrorToastText"] = "Completa todos los campos.";
                return Page();
            }

            if (!string.Equals(Input.Password, Input.ConfirmPassword))
            {
                //ShowErrorToast = true;
                //ErrorToastText = "La confirmación de la contraseña no coincide.";

                TempData["ShowErrorToast"] = true;
                TempData["ErrorToastText"] = "La confirmación de la contraseña no coincide.";

                return Page();
            }

            if (!IsValidEmail(Input.Email))
            {
                TempData["ShowErrorToast"] = true;
                TempData["ErrorToastText"] = "Email inválido.";
                return Page();
            }

            // ¿Usuario ya existe?
            var existing = await _users.GetByUserNameAsync(Input.Username);
            if (existing is not null)
            {

                TempData["ShowErrorToast"] = true;
                TempData["ErrorToastText"] = "El usuario ya existe.";

                return Page();
            }

            // ¿Email ya existe?
            existing = await _users.GetByUserEmailAsync(Input.Email);
            if (existing is not null)
            {
                TempData["ShowErrorToast"] = true;
                TempData["ErrorToastText"] = "El email ya existe.";

                return Page();
            }


            var hash = _passwords.Hash(Input.Password);

            var newUser = new User
            {
                UserName = Input.Username.Trim(),
                Email = Input.Email.Trim(),
                PasswordHash = hash,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                Proveedor = Input.Proveedor.Trim().Replace(".",""),
                Nombre = Input.Nombre,
                Apellido = Input.Apellido
            };

            var ok = await _users.CreateAsync(newUser);
            if (ok == 0)
            {
                TempData["ShowErrorToast"] = true;
                TempData["ErrorToastText"] = "No fue posible crear el usuario.";
             
                return Page();
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            string _connString = config.GetConnectionString("UsersConnection");
            var _urlEncoder = UrlEncoder.Default;
            var _appSettings = config.GetSection("AppSettings").Get<AppSettings>();

            using var conn = new SqlConnection(_connString);
            var userAdmin = await conn.QuerySingleOrDefaultAsync<User>(
                        "SELECT TOP 1 Id, Email, Nombre FROM Users WHERE Proveedor = @Proveedor",
                        new { newUser.Proveedor });

            var subject = "Aviso de aprobación";
            var body = $@"
            Hola {userAdmin.Nombre},
            El usuario {newUser.UserName} {newUser.Apellido} , esta a la espera de aprobación.
            

            Por favor no conteste este correo.";


            var smtpSettings = config.GetSection("SmtpSettings").Get<SmtpSettings>();
            var _emailSender = new SmtpEmailSender(smtpSettings);

            await _emailSender.SendEmailAsync(userAdmin.Email, subject, body);

            ShowSavedModal = true;
            SavedModalText = "Ingresado con éxito, debe ser aceptado para poder ingresar.";

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

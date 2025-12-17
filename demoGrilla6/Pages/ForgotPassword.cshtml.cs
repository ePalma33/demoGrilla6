using Dapper;
using demoGrilla6.Configuration;
using demoGrilla6.Services; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using demoGrilla6.Models;

namespace demoGrilla6.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly string _connString;
        private readonly IEmailSender _emailSender;
        private readonly UrlEncoder _urlEncoder;
        private readonly AppSettings _appSettings;

        public ForgotPasswordModel(
            IConfiguration config,
            IEmailSender emailSender,
            UrlEncoder urlEncoder,
            IOptions<AppSettings> appSettingsOptions)
        {
            _connString = config.GetConnectionString("UsersConnection");
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
            _appSettings = appSettingsOptions.Value;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(string.Empty, "Debes ingresar tu correo.");
                return Page();
            }

            using var conn = new SqlConnection(_connString);
            var user = await conn.QuerySingleOrDefaultAsync<User>(
                "SELECT TOP 1 Id, Email FROM Users WHERE Email = @Email",
                new { Email });

            // Para evitar filtraciones, responde siempre igual aunque no exista
            // Esto mejora la privacidad y evita enumeración de usuarios.
            if (user == null)
            {
                TempData["Info"] = "Si el correo existe, te enviaremos un enlace de recuperación.";
                return RedirectToPage("/ForgotPasswordConfirmation");
            }

            var token = Guid.NewGuid().ToString("N");
            var expires = DateTime.UtcNow.AddMinutes(30);

            await conn.ExecuteAsync(@"
                INSERT INTO PasswordResetTokens (UserId, Token, ExpiresAt, Used)
                VALUES (@UserId, @Token, @ExpiresAt, 0)",
                new { UserId = user.Id, Token = token, ExpiresAt = expires });

            var resetUrl = $"{_appSettings.PublicBaseUrl}/ResetPassword?uid={user.Id}&token={_urlEncoder.Encode(token)}";

            var subject = "Recuperación de contraseña";
            var body = $@"
                Hola,
                Has solicitado recuperar tu contraseña. Usa este enlace (válido por 30 minutos):
                {resetUrl}

                Si no solicitaste este cambio, ignora este correo.";

            await _emailSender.SendEmailAsync(user.Email, subject, body);

            return RedirectToPage("/ForgotPasswordConfirmation");
        }

    }

}




using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;


namespace demoGrilla6.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly string _connString;

        public ResetPasswordModel(IConfiguration config)
        {
            _connString = config.GetConnectionString("UsersConnection");
        }

        [BindProperty(SupportsGet = true)]
        public long Uid { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (Uid <= 0 || string.IsNullOrWhiteSpace(Token))
            {
                return BadRequest("Enlace inválido.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Uid <= 0 || string.IsNullOrWhiteSpace(Token))
            {
                ModelState.AddModelError(string.Empty, "Enlace inválido.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 8)
            {
                ModelState.AddModelError(string.Empty, "La contraseña debe tener al menos 8 caracteres.");
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                return Page();
            }

            using var conn = new SqlConnection(_connString);

            var prt = await conn.QuerySingleOrDefaultAsync<PasswordResetToken>(@"
                SELECT TOP 1 Id, UserId, Token, ExpiresAt, Used
                FROM PasswordResetTokens
                WHERE Token = @Token AND UserId = @UserId",
                new { Token, UserId = Uid });

            if (prt is null || prt.Used || prt.ExpiresAt <= DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "El enlace es inválido o ha expirado.");
                return Page();
            }

            // Hash de la nueva contraseña (ejemplo con SHA256 + salt)
            var salt = Guid.NewGuid().ToString("N");
            var passwordHash = HashPassword(NewPassword, salt);

            await conn.ExecuteAsync(@"
                UPDATE Users
                SET PasswordHash = @PasswordHash
                WHERE Id = @UserId",
                new { PasswordHash = $"{salt}:{passwordHash}", UserId = Uid });

            await conn.ExecuteAsync(@"
                UPDATE PasswordResetTokens
                SET Used = 1
                WHERE Id = @Id",
                new { Id = prt.Id });

            return RedirectToPage("/ResetPasswordConfirmation");
        }

        private static string HashPassword(string password, string salt)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{salt}:{password}"));
            return Convert.ToBase64String(bytes);
        }

        private sealed class PasswordResetToken
        {
            public long Id { get; set; }
            public long UserId { get; set; }
            public string Token { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public bool Used { get; set; }
        }
    }
}




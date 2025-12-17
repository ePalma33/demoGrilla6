using demoGrilla6.Data;
using demoGrilla6.Models;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


namespace demoGrilla6.Pages
{
    public class LoginModel : PageModel
    {

        private readonly IUserRepository _users;
        private readonly IPasswordService _passwords; //  BCrypt
        private readonly IRoleRepository _roles;

        public LoginModel(IUserRepository users, IRoleRepository roles, IPasswordService passwords)
        {
            _users = users;
            _roles = roles;
            _passwords = passwords;
        }


        public string Username { get; set; }

        public string Password { get; set; }

        public string? ErrorMessage { get; set; }


        [BindProperty]
        public InputModel Input { get; set; } = new();
        public string? ReturnUrl { get; set; }



        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }



        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Completa usuario y contraseña.";
                return Page();
            }

            var user = await _users.GetByUserNameAsync(Input.Username);
            if (user is null || !user.IsActive)
            {
                ErrorMessage = "Usuario o contraseña inválidos.";
                return Page();
            }

            // Verificar con BCrypt (NO Identity hasher)
            var ok = _passwords.Verify(Input.Password, user.PasswordHash);
            if (!ok)
            {
                ErrorMessage = "Usuario o contraseña inválidos.";
                return Page();
            }

            // Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("Proveedor", user.Proveedor ?? string.Empty)
            };


            var roleNames = await _roles.GetRolesForUserAsync(user.Id);
            foreach (var role in roleNames)
                claims.Add(new Claim(ClaimTypes.Role, role)); // Evaluable por [Authorize(Roles="...")]


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = Input.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    AllowRefresh = true
                });

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                return Redirect(ReturnUrl);

            TempData["TotalOcPendientes"] = null;

            return RedirectToPage("Index");
        }




        //public IActionResult OnPost()
        //{
        //    //    Username == "admin" && Password == "ASdsdffer1r2y3
        //    //    Username == "10000256-6" && Password == "ASdsdffer1r2y3

        //    // Validación simple (puedes cambiar por tu lógica real)
        //    //(Username == "10000256-6" || Username == "admin") &&
        //    if ( Password == "ASdsdffer1r2y3")
        //    {
        //        HttpContext.Session.SetString("Username", Username);

        //        HttpContext.Session.SetString("IsLoggedIn", "true");
        //        TempData["TotalOcPendientes"] = null;
        //        return RedirectToPage("Index");
        //    }
        //    else
        //    {
        //        ErrorMessage = "Usuario o contraseña incorrectos.";
        //        return Page();
        //    }
        //}
    }




    public class InputModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }



}


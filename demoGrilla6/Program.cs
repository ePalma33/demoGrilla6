using demoGrilla6.Data;
using demoGrilla6.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using System.Data;
using demoGrilla6.Configuration;

var builder = WebApplication.CreateBuilder(args);


// Agregar servicios de autenticación y cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";       // Página de login
        options.LogoutPath = "/Logout";     // Página de logout
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración
        options.Cookie.Name = ".MiApp.Auth";
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            // Opcional: seguridad extra para ReturnUrl
            OnRedirectToLogin = ctx =>
            {
                // Mantener ReturnUrl
                ctx.Response.Redirect($"{ctx.RedirectUri}");
                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddAuthorization();



// Configura SmtpSettings desde appsettings.json (recomendado)
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>() ?? new SmtpSettings
{
    Host = "smtp.tu-proveedor.com",
    Port = 587,
    EnableSsl = true,
    Username = "usuario",
    Password = "clave",
    From = "no-reply@tudominio.com"
};

// Registros DI
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();


builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddRazorPages();



// Registrar Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Index"); // exige login para Index
    // o options.Conventions.AuthorizeFolder("/"); para todo el sitio
});


//  Registrar repositorio y servicio con la cadena de conexión
builder.Services.AddScoped<OrdenCompraCabRepository>(sp =>
    new OrdenCompraCabRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<OrdenCompraDetService>();

builder.Services.AddScoped<OrdenCompraDetRepository>(sp =>
    new OrdenCompraDetRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<OrdenCompraCabService>();

builder.Services.AddScoped<FacturaRepository>(sp =>
    new FacturaRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<FacturaService>();

builder.Services.AddScoped<RecepcionCabRepository>(sp =>
    new RecepcionCabRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<RecepcionCabService>();

builder.Services.AddScoped<RecepcionDetRepository>(sp =>
    new RecepcionDetRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<RecepcionDetService>();

builder.Services.AddScoped<PagoFacturaRepository>(sp =>
    new PagoFacturaRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<PagoFacturaService>();


var cs = builder.Configuration.GetConnectionString("UsersConnection")
         ?? throw new InvalidOperationException("Falta ConnectionStrings:UsersConnection");

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var conn = new SqlConnection(cs);
    conn.Open();
    return conn;
});


// Repositorio de usuarios (Dapper)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordService, BcryptPasswordService>();
builder.Services.AddScoped<IRoleRepository>(sp => new RoleRepository(cs));


// Servicio de dominio para la nueva ventana de Usuarios
builder.Services.AddScoped<UsuarioService>();



builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();



// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
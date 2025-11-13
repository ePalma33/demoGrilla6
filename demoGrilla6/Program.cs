using demoGrilla6.Data;
using demoGrilla6.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar Razor Pages
builder.Services.AddRazorPages();

//  Registrar repositorio y servicio con la cadena de conexión
builder.Services.AddScoped<PurchTableRepository>(sp =>
    new PurchTableRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<PedidoCompraService>();

builder.Services.AddScoped<PurchLineRepository>(sp =>
    new PurchLineRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<PurchLineService>();

builder.Services.AddScoped<VendInvoiceJourRepository>(sp =>
    new VendInvoiceJourRepository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<VendInvoiceJourService>();

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
app.UseAuthorization();
app.MapRazorPages();
app.Run();
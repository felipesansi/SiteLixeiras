using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios;
using SiteLixeiras.Repositorios.Interfaces;
using SiteLixeiras.Sevices;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controllers com views
builder.Services.AddControllersWithViews();

// Configura��o do banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Reposit�rios e servi�os
builder.Services.AddTransient<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddTransient<IProdutosRepositorio, ProdutosRepositorio>();
builder.Services.AddTransient<IPedido, PedidoRepositorio>();
builder.Services.AddScoped<ISeedUserRolesInitial, SeedUserRolesInitial>();

// Adiciona o IHttpContextAccessor (ESSENCIAL para acessar a sess�o no CarrinhoCompra)
builder.Services.AddHttpContextAccessor();

// Configura o carrinho de compras com sess�o
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinhoCompra(sp));

// Upload e Dropbox
builder.Services.AddTransient<IUploadFotosService, UploadFotosService>();
builder.Services.Configure<DropboxSettings>(builder.Configuration.GetSection("Dropbox"));

// Carrega a configura��o do MercadoPago
builder.Services.Configure<MercadoPagoSettings>(builder.Configuration.GetSection("MercadoPago"));

// Sess�o (deve vir ANTES de Authentication/Authorization no pipeline)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "Lixeiras.Session";
});

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();               
app.UseAuthentication();
app.UseAuthorization();

// Cria��o de perfis e usu�rios padr�o
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seed = services.GetRequiredService<ISeedUserRolesInitial>();
    await seed.SeedRolesAsync();
    await seed.SeedUsersAsync();
}

// Rotas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

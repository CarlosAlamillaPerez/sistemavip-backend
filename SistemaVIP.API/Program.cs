using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaVIP.Core.Configuration;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;
using SistemaVIP.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite() // Agrega esta l�nea
    ));

// Add Identity
builder.Services.AddIdentity<ApplicationUserModel, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Authentication & Authorization
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:7198")  // URL de tu aplicaci�n Angular
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .WithExposedHeaders("WWW-Authenticate")
               .SetIsOriginAllowed(origin => true);
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Auth/Login"; // Ruta para iniciar sesi�n
    options.LogoutPath = "/Auth/Logout"; // Ruta para cerrar sesi�n
    options.AccessDeniedPath = "/Auth/AccessDenied"; // Ruta cuando no se tiene acceso
    options.Cookie.Name = "SistemaVIP.Auth"; // Nombre de la cookie
    options.Cookie.HttpOnly = true; // Seguridad para las cookies
    options.Cookie.SameSite = SameSiteMode.None; // Permite enviar cookies entre dominios
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo se env�a en HTTPS
    options.ExpireTimeSpan = TimeSpan.FromHours(8); // Duraci�n de la cookie
});

builder.Services.AddAuthorization();


// Register Services
builder.Services.AddScoped<DbInitializerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IComisionService, ComisionService>();
builder.Services.AddScoped<ITerapeutaService, TerapeutaService>();
builder.Services.AddScoped<IPresentadorService, PresentadorService>();
builder.Services.AddScoped<IValidacionesPresentadorService, ValidacionesPresentadorService>();
builder.Services.AddScoped<ITerapeutasPresentadoresService, TerapeutasPresentadoresService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();
builder.Services.AddScoped<IBlacklistService, BlacklistService>();
builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));
builder.Services.AddHttpClient("CallMeBot", client => { client.DefaultRequestHeaders.Add("User-Agent", "SistemaVIP-WhatsApp/1.0"); });
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IServicioExtraService, ServicioExtraService>();
builder.Services.AddScoped<IReportesService, ReportesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializerService>();
    await initializer.InitializeAsync();
}

app.Run();
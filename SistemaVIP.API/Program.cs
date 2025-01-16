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
        x => x.UseNetTopologySuite() // Agrega esta línea
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

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.Name = "SistemaVIP.Auth";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.ExpireTimeSpan = TimeSpan.FromHours(8);

//    options.Events = new CookieAuthenticationEvents
//    {
//        OnRedirectToLogin = context =>
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            return Task.CompletedTask;
//        },
//        OnRedirectToAccessDenied = context =>
//        {
//            context.Response.StatusCode = StatusCodes.Status403Forbidden;
//            return Task.CompletedTask;
//        }
//    };
//});

// Configure Authentication & Authorization
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:7198")  // Cambiar a HTTPS
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .WithExposedHeaders("WWW-Authenticate")
               .SetIsOriginAllowed(origin => true);
    });
});


// Reemplázala con esta:
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
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

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(); 

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
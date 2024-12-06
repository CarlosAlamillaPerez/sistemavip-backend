using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Models;
using SistemaVIP.Infrastructure.Persistence.Context;

namespace SistemaVIP.Infrastructure.Services
{
    public class DbInitializerService
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public DbInitializerService(
            UserManager<ApplicationUserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (await _context.Database.CanConnectAsync())
                {
                    await InitializeRolesAsync();
                    await InitializeAdministratorsAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error durante la inicialización de la base de datos", ex);
            }
        }

        private async Task InitializeRolesAsync()
        {
            foreach (var roleName in UserRoles.AllRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task InitializeAdministratorsAsync()
        {
            // Crear Super Admin
            await CreateAdministratorAsync(
                "SuperAdmin",
                UserRoles.SUPER_ADMIN);

            // Crear Admin
            await CreateAdministratorAsync(
                "Admin",
                UserRoles.ADMIN);
        }

        private async Task CreateAdministratorAsync(string settingsKey, string role)
        {
            var settings = _configuration.GetSection($"AdminSettings:{settingsKey}");
            var email = settings["Email"];
            var username = settings["Username"];

            var adminUser = await _userManager.FindByEmailAsync(email);
            if (adminUser == null)
            {
                adminUser = new ApplicationUserModel  // Cambiado aquí también
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    NormalizedUserName = username.ToUpper(),
                    NormalizedEmail = email.ToUpper()
                };

                var result = await _userManager.CreateAsync(adminUser, settings["Password"]);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, role);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Error al crear usuario {username}: {errors}");
                }
            }
            else
            {
                // Asegurarse de que el usuario existente tenga el rol correcto
                if (!await _userManager.IsInRoleAsync(adminUser, role))
                {
                    await _userManager.AddToRoleAsync(adminUser, role);
                }
            }
        }
    }
}
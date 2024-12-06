using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SistemaVIP.Core.DTOs.Auth;
using SistemaVIP.Core.Enums;
using SistemaVIP.Core.Interfaces;
using SistemaVIP.Core.Models;

namespace SistemaVIP.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Intentar encontrar usuario por email o username
            var user = await _userManager.FindByEmailAsync(request.Email) ??
                      await _userManager.FindByNameAsync(request.Email);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                };
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                };
            }

            var userDto = await CreateUserDtoAsync(user);

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login exitoso",
                User = userDto
            };
        }

        public async Task<bool> ValidateAuthenticationAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            var user = await _userManager.FindByIdAsync(userId);
            return user != null;
        }

        public async Task LogoutAsync(string userId)
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserDto> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            return await CreateUserDtoAsync(user);
        }

        private async Task<UserDto> CreateUserDtoAsync(ApplicationUserModel user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var (nombre, apellido) = await GetUserNameAndLastNameAsync(user, role);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = role,
                Nombre = nombre,
                Apellido = apellido
            };
        }

        private async Task<(string Nombre, string Apellido)> GetUserNameAndLastNameAsync(ApplicationUserModel user, string role)
        {
            // Para administradores, obtener información de la configuración
            if (role == UserRoles.SUPER_ADMIN || role == UserRoles.ADMIN)
            {
                var settingsKey = role == UserRoles.SUPER_ADMIN ? "SuperAdmin" : "Admin";
                var settings = _configuration.GetSection($"AdminSettings:{settingsKey}");
                return (settings["Nombre"], settings["Apellido"]);
            }

            // Para otros roles, la implementación dependerá de tus tablas
            // Por ahora retornamos valores por defecto
            return (user.UserName, "");
        }
    }
}
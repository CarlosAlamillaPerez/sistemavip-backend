using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs.Auth;
using SistemaVIP.Core.Models;
using SistemaVIP.Web.Interfaces;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaVIP.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly UserManager<ApplicationUserModel> _userManager;

        public AuthController(
            IApiService apiService,
            SignInManager<ApplicationUserModel> signInManager,
            UserManager<ApplicationUserModel> userManager)
        {
            _apiService = apiService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Realiza la llamada al backend para autenticar
                    var result = await _apiService.PostAsync<LoginResponseDto>("api/Auth/login", model);

                    if (result.Success)
                    {
                        // Almacenar la información del usuario en Claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, result.User.Email),
                            new Claim(ClaimTypes.Role, result.User.Role),
                            new Claim(ClaimTypes.NameIdentifier, result.User.Id),
                            new Claim("FullName", $"{result.User.Nombre} {result.User.Apellido}")
                        };

                        // Crear la identidad y principal
                        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Configurar las propiedades de autenticación
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        };

                        // Iniciar sesión usando el esquema correcto
                        await HttpContext.SignInAsync(
                            IdentityConstants.ApplicationScheme,
                            claimsPrincipal,
                            authProperties);

                        // Redirigir según el rol
                        if (result.User.Role == "SUPER_ADMIN" || result.User.Role == "ADMIN")
                        {
                            return RedirectToAction("Presentadores", "Personal");
                        }
                        else if (result.User.Role == "PRESENTADOR")
                        {
                            return RedirectToAction("Index", "Presentador");
                        }

                        // Si hay una URL de retorno y es local, usarla
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return LocalRedirect(returnUrl);
                        }

                        // Por defecto, ir al Home
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, result.Message ?? "Error de autenticación");
                    System.Diagnostics.Debug.WriteLine($"Error de autenticación: {result.Message}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Excepción durante login: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Error al conectar con el servidor: " + ex.Message);
                }
            }

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await _apiService.PostAsync<object>("api/Auth/logout", null);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

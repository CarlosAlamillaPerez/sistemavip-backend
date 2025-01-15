using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs.Auth;
using SistemaVIP.Core.Models;
using SistemaVIP.Web.Interfaces;
using SistemaVIP.Web.Models;
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
                    new Claim("FullName", $"{result.User.Nombre} {result.User.Apellido}")
                };

                        // Crear la identidad y principal
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Configurar las propiedades de autenticación (como "Recordar sesión")
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe, // Sesión persistente si selecciona "Recordarme"
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) // Duración de la sesión
                        };

                        // Iniciar sesión
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

                        // Redirigir según la URL de retorno o al home
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }

                    // Agregar un error al modelo si no se pudo autenticar
                    ModelState.AddModelError(string.Empty, result.Message ?? "Error de autenticación");
                }
                catch (Exception ex)
                {
                    // Error al conectar con el backend
                    ModelState.AddModelError(string.Empty, "Error al conectar con el servidor");
                }
            }

            // Devolver la vista con los errores del modelo
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

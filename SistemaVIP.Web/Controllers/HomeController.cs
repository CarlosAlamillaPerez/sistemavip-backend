using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaVIP.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            // Debug information
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var userName = User.Identity?.Name;
            var claims = User.Claims.ToList();

            if (!isAuthenticated)
            {
                // Si no está autenticado, redirigir explícitamente al login
                return RedirectToAction("Login", "Auth");
            }

            // Si está autenticado, mostrar la vista con la información
            ViewBag.UserEmail = userName;
            ViewBag.UserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            ViewBag.UserFullName = User.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;

            return View();
        }


        // Método para verificar el estado de autenticación via AJAX
        [HttpGet]
        public IActionResult CheckAuth()
        {
            return Json(new
            {
                isAuthenticated = User.Identity.IsAuthenticated,
                username = User.Identity.Name,
                role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            });
        }
    }
}
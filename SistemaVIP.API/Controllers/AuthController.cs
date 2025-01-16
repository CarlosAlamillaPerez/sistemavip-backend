using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVIP.Core.DTOs.Auth;
using SistemaVIP.Core.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace SistemaVIP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (!response.Success)
            {
                return Unauthorized(response);
            }

            // Establecer la cookie de autenticación
            await HttpContext.SignInAsync(
    IdentityConstants.ApplicationScheme,  // Cambiar esto
    new ClaimsPrincipal(new ClaimsIdentity(
        new[] {
            new Claim(ClaimTypes.NameIdentifier, response.User.Id),
            new Claim(ClaimTypes.Name, response.User.Email),
            new Claim(ClaimTypes.Role, response.User.Role)
        },
        IdentityConstants.ApplicationScheme)),  // Cambiar esto también
    new AuthenticationProperties
    {
        IsPersistent = request.RememberMe,
        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
    });

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            await _authService.LogoutAsync(userId);

            // Eliminar la cookie de autenticación
            HttpContext.Response.Cookies.Delete("X-Access-Token");

            return Ok();
        }

        [Authorize]
        [HttpGet("check")]
        public async Task<ActionResult<UserDto>> CheckAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "No autenticado" });
            }

            var user = await _authService.GetCurrentUserAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
        }


    }
}
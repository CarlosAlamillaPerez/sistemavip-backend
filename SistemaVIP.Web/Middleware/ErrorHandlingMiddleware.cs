using System.Net;
using System.Text.Json;

namespace SistemaVIP.Web.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = true,
                message = GetUserFriendlyMessage(exception),
                details = exception.Message
            };

            // Si es una solicitud AJAX, devolver JSON
            if (IsAjaxRequest(context.Request))
            {
                context.Response.StatusCode = (int)GetStatusCode(exception);
                await context.Response.WriteAsJsonAsync(response);
            }
            else
            {
                // Si no es AJAX, redirigir a página de error
                context.Response.Redirect($"/Error?message={WebUtility.UrlEncode(response.message)}");
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                // Agregar más mappings según necesites
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException => "No tiene permisos para realizar esta acción",
                // Agregar más mensajes amigables según el tipo de excepción
                _ => "Ha ocurrido un error inesperado en el sistema"
            };
        }
    }

    // Extension method para configurar el middleware
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
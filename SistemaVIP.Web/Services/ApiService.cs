using SistemaVIP.Web.Controllers;
using SistemaVIP.Web.Interfaces;
using System.Net;
using System.Text.Json;

namespace SistemaVIP.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;


        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;


            // Agregar el handler para las cookies
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }

        //private void AddAuthenticationHeader()
        //{
        //    var cookies = _httpContextAccessor.HttpContext?.Request.Cookies;
        //    if (cookies != null)
        //    {
        //        // Log todas las cookies disponibles
        //        foreach (var cookie in cookies)
        //        {
        //            _logger.LogInformation($"Cookie: {cookie.Key} = {cookie.Value}");
        //        }

        //        // Asegurarnos de enviar la cookie de autenticación
        //        var authCookie = cookies[".AspNetCore.Identity.Application"] ??
        //                        cookies["SistemaVIP.Auth"];  // Verificar ambos nombres de cookie

        //        if (!string.IsNullOrEmpty(authCookie))
        //        {
        //            if (!_httpClient.DefaultRequestHeaders.Contains("Cookie"))
        //            {
        //                _httpClient.DefaultRequestHeaders.Add("Cookie", $".AspNetCore.Identity.Application={authCookie}");
        //            }
        //        }
        //        else
        //        {
        //            _logger.LogWarning("No se encontró cookie de autenticación");
        //        }
        //    }
        //}

        private void AddAuthenticationHeader()
        {
            var cookies = _httpContextAccessor.HttpContext?.Request.Cookies;
            if (cookies != null)
            {
                // Primero, limpiamos cualquier header de Cookie previo
                if (_httpClient.DefaultRequestHeaders.Contains("Cookie"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Cookie");
                }

                // Obtenemos todas las cookies relevantes de autenticación
                var relevantCookies = new List<string>();

                if (cookies.TryGetValue(".AspNetCore.Identity.Application", out string identityCookie))
                {
                    relevantCookies.Add($".AspNetCore.Identity.Application={identityCookie}");
                }

                if (cookies.TryGetValue("SistemaVIP.Auth", out string sistemaCookie))
                {
                    relevantCookies.Add($"SistemaVIP.Auth={sistemaCookie}");
                }

                // Si tenemos cookies, las agregamos al header
                if (relevantCookies.Any())
                {
                    _httpClient.DefaultRequestHeaders.Add("Cookie", string.Join("; ", relevantCookies));
                }
            }
        }

        //public async Task<T> GetAsync<T>(string endpoint)
        //{
        //    AddAuthenticationHeader();
        //    var response = await _httpClient.GetAsync(endpoint);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadFromJsonAsync<T>();
        //    }
        //    throw new HttpRequestException($"Error calling API: {response.StatusCode}");
        //}

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthenticationHeader();

                // Agregamos el header de Accept explícitamente
                if (!_httpClient.DefaultRequestHeaders.Contains("Accept"))
                {
                    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                }

                var response = await _httpClient.GetAsync(endpoint);

                // Si obtenemos unauthorized, vamos a loggear los headers para debug
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var requestHeaders = _httpClient.DefaultRequestHeaders.ToString();
                    // Aquí podrías agregar un log con los headers
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }

                // Intentamos leer el contenido del error para más detalles
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error calling API: {response.StatusCode} - {errorContent}");
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error calling API: {ex.Message}");
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            AddAuthenticationHeader();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);

            // Intentar leer el contenido del cuerpo incluso si la solicitud no es exitosa
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return string.IsNullOrWhiteSpace(content)
                    ? default
                    : JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // Manejo de errores: devolver el mensaje del backend si está disponible
            var errorMessage = !string.IsNullOrWhiteSpace(content) ? content : $"Error {response.StatusCode}";
            throw new HttpRequestException(errorMessage);
        }


        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            AddAuthenticationHeader();
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            throw new HttpRequestException($"Error calling API: {response.StatusCode}");
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            AddAuthenticationHeader();
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
    }
}

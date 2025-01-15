using SistemaVIP.Web.Interfaces;
using System.Text.Json;

namespace SistemaVIP.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            throw new HttpRequestException($"Error calling API: {response.StatusCode}");
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
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
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            throw new HttpRequestException($"Error calling API: {response.StatusCode}");
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
    }
}

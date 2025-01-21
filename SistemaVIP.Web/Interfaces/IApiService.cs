﻿namespace SistemaVIP.Web.Interfaces
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<T>(string endpoint, object data);
        Task<T> PutAsync<T>(string endpoint, object data);
        Task<T> PatchAsync<T>(string endpoint, object data);
        Task<bool> DeleteAsync(string endpoint);
    }

}

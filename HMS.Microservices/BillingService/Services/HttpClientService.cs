using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace BillingService.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpRequestMessage AddAuthHeader(HttpRequestMessage request)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader))
        {
            request.Headers.Add("Authorization", authHeader);
        }
        return request;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var request = AddAuthHeader(new HttpRequestMessage(HttpMethod.Get, url));
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string url, object data)
    {
        var request = AddAuthHeader(new HttpRequestMessage(HttpMethod.Post, url));
        request.Content = JsonContent.Create(data);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task PutAsync(string url, object data)
    {
        var request = AddAuthHeader(new HttpRequestMessage(HttpMethod.Put, url));
        request.Content = JsonContent.Create(data);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string url)
    {
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
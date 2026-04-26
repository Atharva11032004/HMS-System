namespace StaffService.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
    {
        return await _httpClient.PostAsync(url, content);
    }
}
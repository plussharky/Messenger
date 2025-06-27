using System.Net.Http.Json;
using ChatClient.Options;
using Microsoft.JSInterop;

namespace ChatClient.Services;

internal sealed class AuthorizedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly IAuthService _authService;
    private bool _isRefreshing;

    public AuthorizedHttpClient(
        IJSRuntime jsRuntime,
        ApiBaseUrl apiBaseUrl,
        IAuthService authService)
    {
        _jsRuntime = jsRuntime;
        _authService = authService;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(apiBaseUrl.Value),
        };
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T content)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(content),
        };
        return await SendAsync(request);
    }

    public async Task<T?> GetFromJsonAsync<T>(string requestUri)
    {
        var response = await GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        return default;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "access_token");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthorizedHttpClient] Error retrieving token: {ex.Message}");
        }

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !_isRefreshing)
        {
            _isRefreshing = true;

            try
            {
                var refreshSuccess = await _authService.RefreshTokenAsync();

                if (refreshSuccess)
                {
                    var newToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "access_token");
                    if (!string.IsNullOrEmpty(newToken))
                    {
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);
                        response = await _httpClient.SendAsync(request);
                    }
                }
                else
                {
                    await _authService.LogoutAsync();
                    await _jsRuntime.InvokeVoidAsync("window.location.href", "/login");
                }
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        return response;
    }
}

using System.Net.Http.Json;
using ChatClient.Models;
using Microsoft.JSInterop;

namespace ChatClient.Services;

internal sealed class AuthService(IdentityHttpClient httpClient, IJSRuntime jsRuntime)
    : IAuthService
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string IdentityApiRoute = "api/Auth/login";
    private const string RefreshTokenRoute = "api/Auth/refresh";

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(IdentityApiRoute, request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (loginResponse != null)
                {
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, loginResponse.AccessToken);
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, loginResponse.RefreshToken);

                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            var request = new RefreshTokenRequestDto
            {
                RefreshToken = refreshToken,
            };

            var response = await httpClient.PostAsJsonAsync(RefreshTokenRoute, request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (loginResponse != null)
                {
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, loginResponse.AccessToken);
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, loginResponse.RefreshToken);

                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}

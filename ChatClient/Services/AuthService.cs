using System.Net.Http.Json;
using Blazored.LocalStorage;
using ChatClient.Models;
using ChatClient.Options;

namespace ChatClient.Services;

internal sealed class AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
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
            using var httpClient = httpClientFactory.CreateClient(HttpClientNames.IdentityClient);
            var response = await httpClient.PostAsJsonAsync(IdentityApiRoute, request);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResponse == null)
            {
                return false;
            }

            await localStorage.SetItemAsync(AccessTokenKey, loginResponse.AccessToken);
            await localStorage.SetItemAsync(RefreshTokenKey, loginResponse.RefreshToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemAsync(AccessTokenKey);
        await localStorage.RemoveItemAsync(RefreshTokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await localStorage.GetItemAsync<string>(AccessTokenKey);
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await localStorage.GetItemAsync<string>(AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await localStorage.GetItemAsync<string>(RefreshTokenKey);
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await localStorage.GetItemAsync<string>(RefreshTokenKey);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            var request = new RefreshTokenRequestDto
            {
                RefreshToken = refreshToken,
            };

            using var httpClient = httpClientFactory.CreateClient(HttpClientNames.IdentityClient);
            var response = await httpClient.PostAsJsonAsync(RefreshTokenRoute, request);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResponse == null)
            {
                return false;
            }

            await localStorage.SetItemAsync(AccessTokenKey, loginResponse.AccessToken);
            await localStorage.SetItemAsync(RefreshTokenKey, loginResponse.RefreshToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}

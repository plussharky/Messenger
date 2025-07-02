using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace ChatClient.Services;

internal sealed class AuthenticationHandler(IAuthService authService, NavigationManager navigationManager)
    : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphore = new (1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await AddAuthorizationHeaderAsync(request);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var currentToken = await authService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(currentToken))
                {
                    await AddAuthorizationHeaderAsync(request);
                    response = await base.SendAsync(request, cancellationToken);

                    if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                    {
                        return response;
                    }
                }

                var refreshSuccess = await authService.RefreshTokenAsync();

                if (refreshSuccess)
                {
                    await AddAuthorizationHeaderAsync(request);
                    response = await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    await authService.LogoutAsync();
                    navigationManager.NavigateTo("/login");
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return response;
    }

    private async Task AddAuthorizationHeaderAsync(HttpRequestMessage request)
    {
        try
        {
            var token = await authService.GetAccessTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthenticationHandler] Error retrieving token: {ex.Message}");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _semaphore?.Dispose();
        }

        base.Dispose(disposing);
    }
}

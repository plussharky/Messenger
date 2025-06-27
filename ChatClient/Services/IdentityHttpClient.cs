using ChatClient.Options;

namespace ChatClient.Services;

internal sealed class IdentityHttpClient : HttpClient
{
    public IdentityHttpClient(ApiIdentityUrl apiIdentityUrl)
    {
        BaseAddress = new Uri(apiIdentityUrl.Value);
    }
}

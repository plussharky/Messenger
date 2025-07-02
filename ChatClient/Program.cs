using Blazored.LocalStorage;
using ChatClient;
using ChatClient.Options;
using ChatClient.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiIdentityUrl = builder.Configuration["ApiIdentityUrl"]
    ?? throw new InvalidOperationException("ApiIdentityUrl is not configured in appsettings.json");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured in appsettings.json");

builder.Services.AddSingleton(new ApiIdentityUrl() { Value = apiIdentityUrl });
builder.Services.AddSingleton(new ApiBaseUrl() { Value = apiBaseUrl });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<AuthenticationHandler>();
builder.Services.AddScoped<RetryPolicyHandler>();

builder.Services.AddHttpClient(HttpClientNames.IdentityClient, client =>
{
    client.BaseAddress = new Uri(apiIdentityUrl);
})
.AddHttpMessageHandler<RetryPolicyHandler>();

builder.Services.AddHttpClient(HttpClientNames.AuthorizedClient, client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<RetryPolicyHandler>()
.AddHttpMessageHandler<AuthenticationHandler>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();

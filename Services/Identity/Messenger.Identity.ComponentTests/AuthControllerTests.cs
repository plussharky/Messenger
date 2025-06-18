using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Messenger.Identity.Api.Dtos;
using Xunit;

namespace Messenger.Identity.ComponentTests;

public sealed class AuthControllerTests(IdentityWebApplicationFactory factory)
    : IClassFixture<IdentityWebApplicationFactory>, IAsyncLifetime
{
    private const string AuthEndpoint = "/api/auth";

    private readonly HttpClient _client = factory.CreateClient();

    private readonly Fixture _fixture = new ();

    [Fact]
    public async Task Register_WithValidData_ReturnsUserId()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequestDto>();
        request.Email = _fixture.Create<string>() + "@test.com";

        // Act
        var response = await _client.PostAsJsonAsync($"{AuthEndpoint}/register", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = TestData.User.Email,
            Password = TestData.User.Password,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{AuthEndpoint}/register", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = TestData.User.Email,
            Password = TestData.User.Password,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{AuthEndpoint}/login", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = TestData.User.Email,
            Password = TestData.User.WrongPassword,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{AuthEndpoint}/login", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        // Arrange
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = TestData.RefreshToken.Valid,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{AuthEndpoint}/refresh", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_WithUsedToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = TestData.RefreshToken.Used,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"{AuthEndpoint}/refresh", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await factory.ResetDatabaseAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}

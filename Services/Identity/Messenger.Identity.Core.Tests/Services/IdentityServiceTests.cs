using AutoFixture;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Messenger.Identity.Core.Domain.Errors;
using Messenger.Identity.Core.Repository.Entities;
using Messenger.Identity.Core.Services;
using Messenger.Identity.Events;
using Moq;
using Xunit;

namespace Messenger.Identity.Core.Tests.Services;

public sealed class IdentityServiceTests : TestBase
{
    private readonly IdentityService _identityService;

    public IdentityServiceTests()
    {
        _identityService = new IdentityService(
            UserServiceMock.Object,
            TokenServiceMock.Object,
            RefreshTokenServiceMock.Object,
            EventPublisherMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WhenUserFoundAndPasswordValid_ShouldReturnTokensAndPublishEvent()
    {
        // Arrange
        var user = Fixture.Create<User>();
        var refreshToken = Fixture.Create<RefreshToken>();
        var accessToken = Fixture.Create<string>();
        var email = Fixture.Create<string>();
        var password = Fixture.Create<string>();

        UserServiceMock
            .Setup(service => service.AuthenticateUserAsync(email, password))
            .ReturnsAsync(Result.Success<User, LoginError>(user));

        TokenServiceMock
            .Setup(service => service.GenerateAccessToken(user.Id))
            .Returns(accessToken);

        RefreshTokenServiceMock
            .Setup(service => service.CreateAsync(user.Id))
            .ReturnsAsync(Result.Success<RefreshToken, RefreshTokenError>(refreshToken));

        EventPublisherMock
            .Setup(pub => pub.PublishAsync(It.IsAny<UserLoggedIn>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _identityService.LoginAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var loginResponse = result.Value;
        loginResponse.AccessToken.Should().Be(accessToken);
        loginResponse.RefreshToken.Should().Be(refreshToken.Token);

        UserServiceMock.Verify(service => service.AuthenticateUserAsync(email, password), Times.Once);
        TokenServiceMock.Verify(service => service.GenerateAccessToken(user.Id), Times.Once);
        RefreshTokenServiceMock.Verify(service => service.CreateAsync(user.Id), Times.Once);
        EventPublisherMock.Verify(pub => pub.PublishAsync(It.Is<UserLoggedIn>(e => e.Email == email)), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldReturnEmailNotFoundError()
    {
        // Arrange
        var email = Fixture.Create<string>();
        var password = Fixture.Create<string>();

        UserServiceMock
            .Setup(service => service.AuthenticateUserAsync(email, password))
            .ReturnsAsync(Result.Failure<User, LoginError>(LoginError.EmailNotFound));

        // Act
        var result = await _identityService.LoginAsync(email, password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoginError.EmailNotFound);

        UserServiceMock.Verify(service => service.AuthenticateUserAsync(email, password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ShouldReturnInvalidPasswordError()
    {
        // Arrange
        var email = Fixture.Create<string>();
        var password = Fixture.Create<string>();

        UserServiceMock
            .Setup(service => service.AuthenticateUserAsync(email, password))
            .ReturnsAsync(Result.Failure<User, LoginError>(LoginError.InvalidPassword));

        // Act
        var result = await _identityService.LoginAsync(email, password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoginError.InvalidPassword);

        UserServiceMock.Verify(service => service.AuthenticateUserAsync(email, password), Times.Once);
    }
}

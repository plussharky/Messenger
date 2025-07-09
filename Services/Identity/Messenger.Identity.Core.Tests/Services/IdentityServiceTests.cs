using AutoFixture;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Messenger.Common.Events;
using Messenger.Identity.Core.Domain.Errors;
using Messenger.Identity.Core.Repository.Entities;
using Messenger.Identity.Core.Services;
using Moq;
using Xunit;

namespace Messenger.Identity.Core.Tests.Services;

public sealed class IdentityServiceTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly IdentityService _identityService;
    private readonly Fixture _fixture;

    public IdentityServiceTests()
    {
        _userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        _tokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict);
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>(MockBehavior.Strict);
        _eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);
        _fixture = new Fixture();

        _identityService = new IdentityService(
            _userServiceMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _eventPublisherMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WhenUserFoundAndPasswordValid_ShouldReturnTokensAndPublishEvent()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        var userId = _fixture.Create<Guid>();
        var accessToken = _fixture.Create<string>();
        var refreshTokenValue = _fixture.Create<string>();

        var user = _fixture.Build<User>()
            .With(user => user.Id, userId)
            .With(user => user.CreatedAt, DateTimeOffset.UtcNow)
            .Create();

        var refreshToken = _fixture.Build<RefreshToken>()
            .With(token => token.Id, _fixture.Create<Guid>())
            .With(token => token.UserId, userId)
            .With(token => token.Token, refreshTokenValue)
            .With(token => token.ExpiresAt, DateTimeOffset.UtcNow.AddDays(7))
            .With(token => token.CreatedAt, DateTimeOffset.UtcNow)
            .With(token => token.IsRevoked, value: false)
            .Create();

        _userServiceMock
            .Setup(service => service.AuthenticateUserAsync(email, password))
            .ReturnsAsync(Result.Success<User, LoginError>(user));

        _tokenServiceMock
            .Setup(service => service.GenerateAccessToken(userId))
            .Returns(accessToken);

        _refreshTokenServiceMock
            .Setup(service => service.CreateAsync(userId))
            .ReturnsAsync(Result.Success<RefreshToken, RefreshTokenError>(refreshToken));

        _eventPublisherMock
            .Setup(pub => pub.PublishAsync(It.IsAny<UserLoggedIn>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _identityService.LoginAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var loginResponse = result.Value;
        loginResponse.AccessToken.Should().Be(accessToken);
        loginResponse.RefreshToken.Should().Be(refreshTokenValue);

        _userServiceMock.Verify(service => service.AuthenticateUserAsync(email, password), Times.Once);
        _tokenServiceMock.Verify(service => service.GenerateAccessToken(userId), Times.Once);
        _refreshTokenServiceMock.Verify(service => service.CreateAsync(userId), Times.Once);
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.Is<UserLoggedIn>(e => e.Email == email)), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldReturnEmailNotFoundError()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        _userServiceMock
            .Setup(service => service.AuthenticateUserAsync(email, password))
            .ReturnsAsync(Result.Failure<User, LoginError>(LoginError.EmailNotFound));

        // Act
        var result = await _identityService.LoginAsync(email, password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoginError.EmailNotFound);

        _userServiceMock.Verify(service => service.AuthenticateUserAsync(email, password), Times.Once);
        _tokenServiceMock.Verify(service => service.GenerateAccessToken(It.IsAny<Guid>()), Times.Never);
        _refreshTokenServiceMock.Verify(service => service.CreateAsync(It.IsAny<Guid>()), Times.Never);
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.IsAny<UserLoggedIn>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ShouldReturnInvalidPasswordError()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        _userServiceMock
            .Setup(service => service.AuthenticateUserAsync(email, password))
            .ReturnsAsync(Result.Failure<User, LoginError>(LoginError.InvalidPassword));

        // Act
        var result = await _identityService.LoginAsync(email, password);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LoginError.InvalidPassword);

        _userServiceMock.Verify(service => service.AuthenticateUserAsync(email, password), Times.Once);
        _tokenServiceMock.Verify(service => service.GenerateAccessToken(It.IsAny<Guid>()), Times.Never);
        _refreshTokenServiceMock.Verify(service => service.CreateAsync(It.IsAny<Guid>()), Times.Never);
        _eventPublisherMock.Verify(pub => pub.PublishAsync(It.IsAny<UserLoggedIn>()), Times.Never);
    }
}

using AutoFixture;
using Messenger.Identity.Core.Services;
using Moq;

namespace Messenger.Identity.Core.Tests.Services;

internal abstract class TestBase : IDisposable
{
    protected readonly Mock<IUserService> UserServiceMock;
    protected readonly Mock<ITokenService> TokenServiceMock;
    protected readonly Mock<IRefreshTokenService> RefreshTokenServiceMock;
    protected readonly Mock<IEventPublisher> EventPublisherMock;
    protected readonly Fixture Fixture;

    protected TestBase()
    {
        UserServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        TokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict);
        RefreshTokenServiceMock = new Mock<IRefreshTokenService>(MockBehavior.Strict);
        EventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);
        Fixture = new Fixture();
    }

    public void Dispose()
    {
        UserServiceMock.VerifyNoOtherCalls();
        TokenServiceMock.VerifyNoOtherCalls();
        RefreshTokenServiceMock.VerifyNoOtherCalls();
        EventPublisherMock.VerifyNoOtherCalls();
    }
}

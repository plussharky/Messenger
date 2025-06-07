namespace Messenger.Identity.Core.Exceptions;

public sealed class InvalidRefreshTokenException()
    : IdentityException("Недействительный refresh token")
{
}

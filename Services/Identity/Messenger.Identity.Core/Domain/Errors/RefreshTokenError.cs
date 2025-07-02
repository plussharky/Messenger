namespace Messenger.Identity.Core.Domain.Errors;

public enum RefreshTokenError
{
    InvalidToken,
    TokenExpired,
    UserNotFound,
    TokenNotFound,
    TokenRevoked,
}

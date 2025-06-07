namespace Messenger.Identity.Core.Exceptions;

public sealed class InvalidCredentialsException : IdentityException
{
    public InvalidCredentialsException()
        : base("Неверный email или пароль")
    {
    }
}

namespace Messenger.Identity.Core.Exceptions;

public sealed class RegistrationFailedException(string message, Exception? inner = null)
    : IdentityException(message, inner)
{
}

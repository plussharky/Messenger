namespace Messenger.Identity.Core.Exceptions;

public abstract class IdentityException(string message)
    : Exception(message)
{
}

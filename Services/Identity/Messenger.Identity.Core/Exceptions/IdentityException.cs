namespace Messenger.Identity.Core.Exceptions;

public abstract class IdentityException : Exception
{
    protected IdentityException(string message)
        : base(message)
    {
    }

    protected IdentityException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

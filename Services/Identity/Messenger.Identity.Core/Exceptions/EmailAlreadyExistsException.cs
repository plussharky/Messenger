namespace Messenger.Identity.Core.Exceptions;

public sealed class EmailAlreadyExistsException(string email)
    : IdentityException($"Пользователь с email {email} уже существует")
{
    public string Email { get; } = email;
}

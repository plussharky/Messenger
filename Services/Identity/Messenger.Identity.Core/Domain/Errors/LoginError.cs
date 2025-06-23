namespace Messenger.Identity.Core.Domain.Errors;

public enum LoginError
{
    EmailNotFound,
    InvalidPassword,
    TokenGenerationFailed,
}

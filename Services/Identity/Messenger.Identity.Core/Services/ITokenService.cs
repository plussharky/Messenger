namespace Messenger.Identity.Core.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId);
}

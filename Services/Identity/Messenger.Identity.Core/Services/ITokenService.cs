using System.Security.Claims;

namespace Messenger.Identity.Core.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();
}

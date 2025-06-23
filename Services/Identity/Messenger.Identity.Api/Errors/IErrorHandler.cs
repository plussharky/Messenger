using Messenger.Identity.Core.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Identity.Api.Errors;

public interface IErrorHandler
{
    IActionResult Handle(RegisterError error);

    IActionResult Handle(LoginError error);

    IActionResult Handle(RefreshTokenError error);
}

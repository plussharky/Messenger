namespace Messenger.Identity.Core.Services;

public interface IUserService
{
    Task<Guid> RegisterUserAsync(string email, string password);

    Task<bool> IsEmailExistsAsync(string email);
}

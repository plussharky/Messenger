using Dapper;
using Messenger.Identity.Core.Exceptions;
using Npgsql;

namespace Messenger.Identity.Core.Services;

internal sealed class UserService(
    string connectionString,
    ITimeProvider timeProvider)
    : IUserService
{
    public async Task<bool> IsEmailExistsAsync(string email)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        return await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM user_credentials WHERE email = @Email)",
            new { Email = email });
    }

    public async Task<Guid> RegisterUserAsync(string email, string password)
    {
        if (await IsEmailExistsAsync(email))
        {
            throw new EmailAlreadyExistsException(email);
        }

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var userId = Guid.NewGuid();
            var createdAt = timeProvider.GetCurrentTime();

            await connection.ExecuteAsync(
                "INSERT INTO users (id, created_at) " +
                "VALUES (@Id, @CreatedAt)",
                new { Id = userId, CreatedAt = createdAt },
                transaction);

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            await connection.ExecuteAsync(
                "INSERT INTO user_credentials (user_id, email, password_hash, salt) " +
                "VALUES (@UserId, @Email, @PasswordHash, @Salt)",
                new { UserId = userId, Email = email, PasswordHash = passwordHash, Salt = salt },
                transaction);

            await transaction.CommitAsync();
            return userId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

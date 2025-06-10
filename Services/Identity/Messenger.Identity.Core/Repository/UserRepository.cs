using Dapper;
using Messenger.Identity.Core.Repository.Entities;

namespace Messenger.Identity.Core.Repository;

internal sealed class UserRepository(IDbConnectionFactory connectionFactory)
    : IUserRepository
{
    public async Task<bool> IsEmailExistsAsync(string email)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM user_credentials WHERE email = @Email)",
            new
            {
                Email = email,
            });
    }

    public async Task<Guid> CreateUserAsync(string email, string passwordHash, DateTimeOffset createdAt)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var userId = Guid.NewGuid();
            await connection.ExecuteAsync(
                "INSERT INTO users (id, created_at) VALUES (@Id, @CreatedAt)",
                new
                {
                    Id = userId,
                    CreatedAt = createdAt,
                },
                transaction);
            await connection.ExecuteAsync(
                "INSERT INTO user_credentials (user_id, email, password_hash) VALUES (@UserId, @Email, @PasswordHash)",
                new
                {
                    UserId = userId,
                    Email = email,
                    PasswordHash = passwordHash,
                },
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

    public async Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<UserCredentials>(
            "SELECT user_id, email, password_hash FROM user_credentials WHERE email = @Email",
            new
            {
                Email = email,
            });
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT id, created_at FROM users WHERE id = @Id",
            new
            {
                Id = userId,
            });
    }
}

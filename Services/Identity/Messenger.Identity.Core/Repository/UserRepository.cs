using Dapper;
using Messenger.Identity.Core.Options;
using Messenger.Identity.Core.Repository.Entities;
using Npgsql;

namespace Messenger.Identity.Core.Repository;

internal sealed class UserRepository(ConnectionString connectionString)
    : IUserRepository
{
    public async Task<bool> IsEmailExistsAsync(string email)
    {
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM user_credentials WHERE email = @Email)",
            new
            {
                Email = email,
            });
    }

    public async Task<Guid> CreateUserAsync(string email, string passwordHash, string salt, DateTimeOffset createdAt)
    {
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
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
                "INSERT INTO user_credentials (user_id, email, password_hash, salt) VALUES (@UserId, @Email, @PasswordHash, @Salt)",
                new
                {
                    UserId = userId,
                    Email = email,
                    PasswordHash = passwordHash,
                    Salt = salt,
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
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
        return await connection.QueryFirstOrDefaultAsync<UserCredentials>(
            "SELECT user_id, email, password_hash, salt FROM user_credentials WHERE email = @Email",
            new
            {
                Email = email,
            });
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        using var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT id, created_at FROM users WHERE id = @Id",
            new
            {
                Id = userId,
            });
    }
}

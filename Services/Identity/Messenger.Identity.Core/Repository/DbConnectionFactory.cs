using Messenger.Identity.Core.Options;
using Npgsql;

namespace Messenger.Identity.Core.Repository;

internal sealed class DbConnectionFactory(ConnectionString connectionString)
    : IDbConnectionFactory
{
    public async Task<NpgsqlConnection> OpenConnectionAsync()
    {
        var connection = new NpgsqlConnection(connectionString.Value);
        await connection.OpenAsync();
        return connection;
    }
} 
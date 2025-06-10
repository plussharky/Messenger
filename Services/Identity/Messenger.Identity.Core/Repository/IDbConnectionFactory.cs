using Npgsql;

namespace Messenger.Identity.Core.Repository;

internal interface IDbConnectionFactory
{
    Task<NpgsqlConnection> OpenConnectionAsync();
} 
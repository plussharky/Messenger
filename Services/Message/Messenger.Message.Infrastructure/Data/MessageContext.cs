using Microsoft.EntityFrameworkCore;

namespace Messenger.Message.Infrastructure.Data;

internal sealed class MessageContext : DbContext
{
    public MessageContext(DbContextOptions<MessageContext> options)
        : base(options)
    {
    }

    required public DbSet<Domain.Entities.Message> Messages { get; set; }
}

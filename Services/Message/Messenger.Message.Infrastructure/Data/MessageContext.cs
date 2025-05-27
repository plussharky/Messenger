using Messenger.Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Messages.Infrastructure.Data;

internal sealed class MessageContext : DbContext
{
    public MessageContext(DbContextOptions<MessageContext> options)
        : base(options)
    {
        Messages = Set<Message>();
    }

    public DbSet<Message> Messages { get; set; }
}

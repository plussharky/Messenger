using Microsoft.EntityFrameworkCore;
namespace Messenger.Message.Infrastructure.Data;

public class MessageContext : DbContext
{
    public MessageContext(DbContextOptions<MessageContext> options) : base(options)
    {
    }

    public required DbSet<Domain.Entities.MessageEntity> Messages { get; set; }
}
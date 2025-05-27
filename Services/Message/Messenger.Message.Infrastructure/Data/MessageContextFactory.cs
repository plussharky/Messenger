using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Messenger.Messages.Infrastructure.Data;

internal sealed class MessageContextFactory : IDesignTimeDbContextFactory<MessageContext>
{
    public MessageContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MessageContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=messengerdb;Username=postgres;Password=postgres");
        return new MessageContext(optionsBuilder.Options);
    }
}

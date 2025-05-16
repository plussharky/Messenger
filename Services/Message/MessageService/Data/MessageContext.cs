using MessageService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Data;

internal class MessageContext : DbContext
{
    public MessageContext(DbContextOptions<MessageContext> options)
        : base(options) { }

    public required DbSet<Message> Messages { get; set; }
}
using MessageService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Data;

public class MessageContext : DbContext
{
    public MessageContext(DbContextOptions<MessageContext> options)
        : base(options) { }

    public DbSet<Message> Messages { get; set; }
}
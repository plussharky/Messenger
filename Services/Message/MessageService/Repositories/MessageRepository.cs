using MessageService.Data;
using MessageService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Repositories;

internal class MessageRepository : IMessageRepository
{
    private readonly MessageContext _messageContext;

    public MessageRepository(MessageContext context)
    {
        _messageContext = context;
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await _messageContext.Messages
                         .AsNoTracking()
                         .OrderByDescending(m => m.SentAt)
                         .ToListAsync();
    }

    public async Task<Message> AddAsync(Message message)
    {
        var entry = await _messageContext.Messages.AddAsync(message);
        await _messageContext.SaveChangesAsync();
        return entry.Entity;
    }
}

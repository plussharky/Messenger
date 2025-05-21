using Messenger.Message.Domain.Entities;
using Messenger.Message.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Message.Infrastructure.Data;

internal sealed class MessageRepository : IMessageRepository
{
    private readonly MessageContext _messageContext;

    public MessageRepository(MessageContext context)
    {
        _messageContext = context;
    }

    public async Task<IEnumerable<MessageEntity>> GetAllAsync()
    {
        return await _messageContext.Messages
                         .AsNoTracking()
                         .OrderByDescending(m => m.SentAt)
                         .ToListAsync();
    }

    public async Task<MessageEntity> AddAsync(MessageEntity message)
    {
        var entry = await _messageContext.Messages.AddAsync(message);
        await _messageContext.SaveChangesAsync();
        return entry.Entity;
    }
}

using Messenger.Message.Domain.Entities;
using Messenger.Message.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Message.Infrastructure.Data;

internal sealed class MessageRepository(MessageContext context)
    : IMessageRepository
{
    private readonly MessageContext _messageContext = context;

    public async Task<IReadOnlyList<MessageEntity>> GetAllAsync()
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

using Messenger.Messages.Domain.Entities;
using Messenger.Messages.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Messages.Infrastructure.Data;

internal sealed class MessageRepository(MessageContext context)
    : IMessageRepository
{
    private readonly MessageContext _messageContext = context;

    public async Task<IReadOnlyList<Message>> GetAllAsync()
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

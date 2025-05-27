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

    public async Task<Message> CreateAsync(Message message)
    {
        var entry = await _messageContext.Messages.AddAsync(message);
        await _messageContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Message?> GetMessageAsync(Guid id)
    {
        return await _messageContext.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Message> UpdateAsync(Message message)
    {
        var entry = _messageContext.Messages.Update(message);
        await _messageContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task DeleteAsync(Message message)
    {
        _messageContext.Messages.Remove(message);
        await _messageContext.SaveChangesAsync();
    }
}

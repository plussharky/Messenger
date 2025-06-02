using Messenger.Messages.Domain.Entities;
using Messenger.Messages.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Messages.Infrastructure.Data;

internal sealed class MessageRepository(MessageContext context)
    : IMessageRepository
{
    private readonly MessageContext _messageContext = context;

    public async Task CreateAsync(Message message)
    {
        await _messageContext.Messages.AddAsync(message);
        await _messageContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Message>> GetAllAsync()
    {
        return await _messageContext.Messages
            .AsNoTracking()
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<Message?> GetMessageAsync(Guid id)
    {
        return await _messageContext.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task UpdateAsync(Message message)
    {
        _messageContext.Messages.Update(message);
        await _messageContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _messageContext.Messages
            .Where(m => m.Id == id)
            .ExecuteDeleteAsync();
    }
}

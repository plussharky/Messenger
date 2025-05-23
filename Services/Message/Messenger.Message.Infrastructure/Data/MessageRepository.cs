using Messenger.Message.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Message.Infrastructure.Data;

internal sealed class MessageRepository(MessageContext context)
    : IMessageRepository
{
    private readonly MessageContext _messageContext = context;

    public async Task<IReadOnlyList<Domain.Entities.Message>> GetAllAsync()
    {
        return await _messageContext.Messages
                         .AsNoTracking()
                         .OrderByDescending(m => m.SentAt)
                         .ToListAsync();
    }

    public async Task<Domain.Entities.Message> AddAsync(Domain.Entities.Message message)
    {
        var entry = await _messageContext.Messages.AddAsync(message);
        await _messageContext.SaveChangesAsync();
        return entry.Entity;
    }
}

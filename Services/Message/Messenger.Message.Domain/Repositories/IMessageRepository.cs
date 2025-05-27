using Messenger.Messages.Domain.Entities;

namespace Messenger.Messages.Domain.Repositories;

public interface IMessageRepository
{
    Task<Message> CreateAsync(Message message);

    Task<Message?> GetMessageAsync(Guid id);

    Task<Message> UpdateAsync(Message message);

    Task DeleteAsync(Message message);

    Task<IReadOnlyList<Message>> GetAllAsync();
}

using Messenger.Messages.Domain.Entities;

namespace Messenger.Messages.Domain.Repositories;

public interface IMessageRepository
{
    Task CreateAsync(Message message);

    Task<Message?> GetMessageAsync(Guid id);

    Task<IReadOnlyList<Message>> GetAllAsync();

    Task UpdateAsync(Message message);

    Task DeleteAsync(Guid id);
}

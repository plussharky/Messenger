using Messenger.Messages.Domain.Entities;

namespace Messenger.Messages.Domain.Repositories;

public interface IMessageRepository
{
    Task<Message> AddAsync(Message message);

    Task<IReadOnlyList<Message>> GetAllAsync();
}

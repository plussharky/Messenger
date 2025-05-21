using Messenger.Message.Domain.Entities;

namespace Messenger.Message.Domain.Repositories;

public interface IMessageRepository
{
    Task<MessageEntity> AddAsync(MessageEntity message);

    Task<IEnumerable<MessageEntity>> GetAllAsync();
}

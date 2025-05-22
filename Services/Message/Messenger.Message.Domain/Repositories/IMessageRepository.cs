using Messenger.Message.Domain.Entities;

namespace Messenger.Message.Domain.Repositories;

public interface IMessageRepository
{
    public Task<MessageEntity> AddAsync(MessageEntity message);

    public Task<IReadOnlyList<MessageEntity>> GetAllAsync();
}

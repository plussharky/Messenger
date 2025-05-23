namespace Messenger.Message.Domain.Repositories;

public interface IMessageRepository
{
    Task<Entities.Message> AddAsync(Entities.Message message);

    Task<IReadOnlyList<Entities.Message>> GetAllAsync();
}

namespace Messenger.Message.Domain.Repositories;

public interface IMessageRepository
{
    public Task<Entities.Message> AddAsync(Entities.Message message);

    public Task<IReadOnlyList<Entities.Message>> GetAllAsync();
}

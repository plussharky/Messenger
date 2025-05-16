using MessageService.Data.Models;

namespace MessageService.Repositories;

public interface IMessageRepository
{
    Task<Message> AddAsync(Message message);
    Task<IEnumerable<Message>> GetAllAsync();
}
using Messenger.Message.Application.Requests;

namespace Messenger.Message.Application.Services;

public interface IMessageService
{
    public Task<IReadOnlyList<Domain.Entities.Message>> GetAllMessagesAsync();

    public Task<Domain.Entities.Message> SendMessageAsync(CreateMessageRequest createMessageDto);
}

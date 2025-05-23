using Messenger.Message.Application.Requests;

namespace Messenger.Message.Application.Services;

public interface IMessageService
{
    Task<IReadOnlyList<Domain.Entities.Message>> GetAllMessagesAsync();

    Task<Domain.Entities.Message> SendMessageAsync(CreateMessageRequest createMessageDto);
}

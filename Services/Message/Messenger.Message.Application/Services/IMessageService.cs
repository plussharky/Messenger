using Messenger.Messages.Application.Requests;
using Messenger.Messages.Domain.Entities;

namespace Messenger.Messages.Application.Services;

public interface IMessageService
{
    Task<IReadOnlyList<Message>> GetAllMessagesAsync();

    Task<Message> SendMessageAsync(CreateMessageRequest createMessageRequest);
}

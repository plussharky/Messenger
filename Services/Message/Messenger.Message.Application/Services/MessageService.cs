using Messenger.Messages.Application.Requests;
using Messenger.Messages.Domain.Entities;
using Messenger.Messages.Domain.Repositories;

namespace Messenger.Messages.Application.Services;

internal sealed class MessageService(IMessageRepository messageRepository, ITimeProvider timeProvider)
    : IMessageService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly ITimeProvider _timeProvider = timeProvider;

    public async Task<IReadOnlyList<Message>> GetAllMessagesAsync()
    {
        return await _messageRepository.GetAllAsync();
    }

    public async Task<Message> SendMessageAsync(SendMessageRequest sendMessageRequest)
    {
        var message = await _messageRepository.GetMessageAsync(sendMessageRequest.Id);

        if (message == null)
        {
            message = new Message(
                sendMessageRequest.Id,
                sendMessageRequest.Text,
                _timeProvider.GetCurrentTime());

            await _messageRepository.CreateAsync(message);
        }
        else if (message.Text != sendMessageRequest.Text)
        {
            message.Text = sendMessageRequest.Text;
            message.UpdatedAt = _timeProvider.GetCurrentTime();
            await _messageRepository.UpdateAsync(message);
        }

        return message;
    }
}

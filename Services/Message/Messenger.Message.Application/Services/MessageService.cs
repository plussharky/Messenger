using AutoMapper;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Domain.Entities;
using Messenger.Messages.Domain.Repositories;

namespace Messenger.Messages.Application.Services;

internal sealed class MessageService(IMessageRepository messageRepository, IMapper mapper, ITimeProvider timeProvider)
    : IMessageService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ITimeProvider _timeProvider = timeProvider;

    public async Task<IReadOnlyList<Message>> GetAllMessagesAsync()
    {
        var messages = await _messageRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<Message>>(messages);
    }

    public async Task<Message> SendMessageAsync(CreateMessageRequest createMessageRequest)
    {
        var message = new Message(
            text: createMessageRequest.Text,
            sentAt: _timeProvider.GetCurrentTime());

        var savedMessage = await _messageRepository.AddAsync(message);
        return _mapper.Map<Message>(savedMessage);
    }
}

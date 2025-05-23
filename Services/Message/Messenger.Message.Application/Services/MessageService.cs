using AutoMapper;
using Messenger.Message.Application.Requests;
using Messenger.Message.Domain.Repositories;

namespace Messenger.Message.Application.Services;

internal sealed class MessageService(IMessageRepository messageRepository, IMapper mapper, ITimeProvider timeProvider)
    : IMessageService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ITimeProvider _timeProvider = timeProvider;

    public async Task<IReadOnlyList<Domain.Entities.Message>> GetAllMessagesAsync()
    {
        var messages = await _messageRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<Domain.Entities.Message>>(messages);
    }

    public async Task<Domain.Entities.Message> SendMessageAsync(CreateMessageRequest createMessageDto)
    {
        var messageEntity = _mapper.Map<Domain.Entities.Message>(createMessageDto);

        messageEntity.SentAt = _timeProvider.GetCurrentTime();

        var savedMessage = await _messageRepository.AddAsync(messageEntity);
        return _mapper.Map<Domain.Entities.Message>(savedMessage);
    }
}

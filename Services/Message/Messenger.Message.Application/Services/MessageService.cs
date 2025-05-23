using AutoMapper;
using Messenger.Message.Application.DTOs;
using Messenger.Message.Domain.Repositories;

namespace Messenger.Message.Application.Services;

internal sealed class MessageService(IMessageRepository messageRepository, IMapper mapper, ITimeProvider timeProvider)
    : IMessageService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ITimeProvider _timeProvider = timeProvider;

    public async Task<IReadOnlyList<MessageDto>> GetAllMessagesAsync()
    {
        var messages = await _messageRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<MessageDto>>(messages);
    }

    public async Task<MessageDto> SendMessageAsync(CreateMessageDto createMessageDto)
    {
        var messageEntity = _mapper.Map<Domain.Entities.MessageEntity>(createMessageDto);

        messageEntity.SentAt = _timeProvider.GetCurrentTime();

        var savedMessage = await _messageRepository.AddAsync(messageEntity);
        return _mapper.Map<MessageDto>(savedMessage);
    }
}

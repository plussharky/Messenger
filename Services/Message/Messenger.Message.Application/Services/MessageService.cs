using AutoMapper;
using Messenger.Message.Application.DTOs;
using Messenger.Message.Domain.Repositories;

namespace Messenger.Message.Application.Services;

public sealed class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly ITimeProvider _timeProvider;

    public MessageService(IMessageRepository messageRepository, IMapper mapper, ITimeProvider timeProvider)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

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

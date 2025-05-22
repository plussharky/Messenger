using Messenger.Message.Application.DTOs;

namespace Messenger.Message.Application.Services;

public interface IMessageService
{
    public Task<IReadOnlyList<MessageDto>> GetAllMessagesAsync();

    public Task<MessageDto> SendMessageAsync(CreateMessageDto createMessageDto);
}

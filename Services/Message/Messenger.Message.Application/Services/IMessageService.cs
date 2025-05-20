using System.Collections.Generic;
using System.Threading.Tasks;
using Messenger.Message.Application.DTOs;

namespace Messenger.Message.Application.Services;

public interface IMessageService
{
    Task<IEnumerable<MessageDto>> GetAllMessagesAsync();
    Task<MessageDto> SendMessageAsync(CreateMessageDto createMessageDto);
} 
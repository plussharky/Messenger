using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Messenger.Message.Application.DTOs;
using Messenger.Message.Domain.Repositories;

namespace Messenger.Message.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MessageDto>> GetAllMessagesAsync()
        {
            var messages = await _messageRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<MessageDto> SendMessageAsync(CreateMessageDto createMessageDto)
        {
            var currentSenderId = Guid.NewGuid();

            var messageEntity = _mapper.Map<Domain.Entities.MessageEntity>(createMessageDto);
            
            messageEntity.SentAt = DateTime.UtcNow;

            var savedMessage = await _messageRepository.AddAsync(messageEntity);
            return _mapper.Map<MessageDto>(savedMessage);
        }
    }
} 
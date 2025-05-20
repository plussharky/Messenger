using AutoMapper;
using Messenger.Message.Application.DTOs;
using Messenger.Message.Domain.Entities;

namespace Messenger.Message.Application.Mappings
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<MessageEntity, MessageDto>();
            CreateMap<CreateMessageDto, MessageEntity>();
        }
    }
} 
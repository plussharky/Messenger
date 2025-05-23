using AutoMapper;
using Messenger.Messages.Api.DTOs;
using Messenger.Messages.Domain.Entities;

namespace Messenger.Messages.Api.Mappings;

internal sealed class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageDto>();
    }
}

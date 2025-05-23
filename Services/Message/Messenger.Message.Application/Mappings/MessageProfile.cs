using AutoMapper;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Domain.Entities;

namespace Messenger.Messages.Application.Mappings;

internal sealed class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<CreateMessageRequest, Message>();
    }
}

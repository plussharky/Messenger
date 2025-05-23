using AutoMapper;
using Messenger.Message.Application.Requests;

namespace Messenger.Message.Api.Mappings;

internal sealed class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Domain.Entities.Message, MessageDto>();
    }
}

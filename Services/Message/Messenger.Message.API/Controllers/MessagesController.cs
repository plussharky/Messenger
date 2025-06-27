using AutoMapper;
using Messenger.Messages.Api.DTOs;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Messages.Api.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize(AuthenticationSchemes = "Bearer")]
public sealed class MessagesController(IMessageService messageService, IMapper mapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<MessageDto>> Get()
    {
        var messages = await messageService.GetAllMessagesAsync();
        return mapper.Map<IReadOnlyList<MessageDto>>(messages);
    }

    [HttpPut("{messageId:guid}")]
    public async Task<IActionResult> Upsert(Guid messageId, [FromBody] CreateMessageRequestDto request)
    {
        var message = new SendMessageRequest()
        {
            Id = messageId,
            Text = request.Text,
        };
        var created = await messageService.SendMessageAsync(message);

        return Ok(mapper.Map<MessageDto>(created));
    }
}

using AutoMapper;
using Messenger.Messages.Api.DTOs;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Messages.Api.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController(IMessageService messageService, IMapper mapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<MessageDto>> Get()
    {
        var messages = await messageService.GetAllMessagesAsync();
        return mapper.Map<IReadOnlyList<MessageDto>>(messages);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateMessageRequestDto request)
    {
        var message = new CreateMessageRequest() { Text = request.Text };
        var created = await messageService.SendMessageAsync(message);
        return Ok(created);
    }
}

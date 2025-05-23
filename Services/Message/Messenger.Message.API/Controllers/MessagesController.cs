using AutoMapper;
using Messenger.Messages.Api.Requests;
using Messenger.Messages.Application.Requests;
using Messenger.Messages.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Messages.Api.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController(IMessageService messageService, IMapper mapper)
    : ControllerBase
{
    private readonly IMessageService _messageService = messageService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<IReadOnlyList<MessageDto>> Get()
    {
        var messages = await _messageService.GetAllMessagesAsync();
        return _mapper.Map<IReadOnlyList<MessageDto>>(messages);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateMessageRequestDto request)
    {
        var message = new CreateMessageRequest() { Text = request.Text };
        var created = await _messageService.SendMessageAsync(message);
        return Ok(created);
    }
}

using Messenger.Message.API.Requests;
using Messenger.Message.Application.DTOs;
using Messenger.Message.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Message.API.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<IEnumerable<MessageDto>> Get()
    {
        return await _messageService.GetAllMessagesAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateMessageRequest request)
    {
        var message = new CreateMessageDto() { Text = request.Text };
        var created = await _messageService.SendMessageAsync(message);
        return Ok(created);
    }
}

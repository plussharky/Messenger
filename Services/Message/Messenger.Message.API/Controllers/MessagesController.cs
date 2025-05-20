using Messenger.Message.API.Requests;
using Messenger.Message.Application.DTOs;
using Messenger.Message.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Message.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IEnumerable<MessageDto>> Get()
        {
            var messages = await _messageService.GetAllMessagesAsync();
            return messages;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateMessageRequest request)
        {
            var message = new CreateMessageDto() { Text = request.Text};
            var created = await _messageService.SendMessageAsync(message);
            return Ok(created);
        }
    }
}

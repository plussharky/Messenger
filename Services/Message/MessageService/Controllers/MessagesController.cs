using MessageService.Data.Models;
using MessageService.DTOs;
using MessageService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public sealed class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessagesController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<MessageDto>> Get()
        {
            var messages = await _messageRepository.GetAllAsync();
            return messages.Select(MessageDto.FromMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateMessageDto dto)
        {
            var message = new Message(dto.Text);
            var created = await _messageRepository.AddAsync(message);
            var createdDto = MessageDto.FromMessage(created);
            return CreatedAtAction(nameof(Get), new { id = createdDto.Id }, createdDto);
        }
    }
}

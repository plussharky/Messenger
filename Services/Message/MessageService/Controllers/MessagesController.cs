using MessageService.Data.Models;
using MessageService.DTOs;
using MessageService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessagesController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Message>> Get()
            => await _messageRepository.GetAllAsync();

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateMessageDto dto)
        {
            var message = new Message
            {
                Text = dto.Text
            };
            
            var created = await _messageRepository.AddAsync(message);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
    }
}

using MessageService.Data.Models;
using MessageService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Post([FromBody] Message msg)
        {
            var created = await _messageRepository.AddAsync(msg);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
    }
}

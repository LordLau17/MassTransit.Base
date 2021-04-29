using Dto;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public MessageController(ILogger<MessageController> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Message message)
        {
            _logger.LogInformation("Published message: {Message}", JsonSerializer.Serialize(message));
            await _publishEndpoint.Publish<Message>(message);

            return Ok();
        }
    }
}
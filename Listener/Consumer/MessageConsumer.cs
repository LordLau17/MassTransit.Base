using Dto;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Listener.Consumer
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var consumerName = nameof(MessageConsumer);
            var messageContent = JsonSerializer.Serialize(context.Message);

            await Task.Run(() =>
            {
                _logger.LogWarning("{Listener} consumed {Message}.", consumerName, messageContent);
                Console.WriteLine($"{consumerName} consumed ${messageContent}");
            });
        }
    }
}
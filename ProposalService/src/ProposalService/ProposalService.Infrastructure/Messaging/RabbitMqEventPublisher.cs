using ProposalService.Application.Ports;
using RabbitMQ.Client;
using System.Text.Json;

namespace ProposalService.Infrastructure.Messaging
{


    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IConnection _connection;

        public RabbitMqEventPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public Task PublishAsync<T>(T @event, string exchangeName)
        {
            using var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);

            var message = JsonSerializer.Serialize(@event);
            var body = System.Text.Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: string.Empty,
                basicProperties: null,
                body: body);

            return Task.CompletedTask;
        }
    }
}

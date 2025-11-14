using EF = Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using ContractService.Infrastructure.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;
using ContractService.Application.Ports;


namespace ContractService.Infrastructure.Messaging.Consumers
{
    public class ProposalStatusChangedConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private IModel _channel;

        public ProposalStatusChangedConsumer(IServiceProvider serviceProvider, IConnection connection)
        {
            _serviceProvider = serviceProvider;
            _connection = connection;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: "proposal.status.changed",
                type: ExchangeType.Fanout,
                durable: true);

            _channel.QueueDeclare(
                queue: "contract.proposalstatus",
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.QueueBind(
                queue: "contract.proposalstatus",
                exchange: "proposal.status.changed",
                routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (sender, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.Span);
                    var message = JsonSerializer.Deserialize<ProposalStatusChangedIntegrationEvent>(json);

                    if (message is not null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var repository = scope.ServiceProvider.GetRequiredService<IProposalStatusCacheRepository>();

                        await repository.UpdateStatusAsync(message.ProposalId, message.NewStatus);
                    }

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception)
                {
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(
                queue: "contract.proposalstatus",
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}

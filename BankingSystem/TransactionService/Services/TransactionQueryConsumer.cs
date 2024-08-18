/*
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TransactionService.Models;

namespace TransactionService.Services
{
    public class TransactionQueryConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public TransactionQueryConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "TransactionQueryQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "TransactionResultQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("I am here");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var queryMessage = JsonSerializer.Deserialize<TransactionQueryMessage>(message);

                if (queryMessage != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
                        var transactions = await transactionService.GetTransactionsByAccountIdAsync(queryMessage.AccountId);

                        var resultMessage = new TransactionResultMessage
                        {
                            Transactions = transactions
                        };

                        var resultBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(resultMessage));
                        _channel.BasicPublish(exchange: "", routingKey: "TransactionResultQueue", basicProperties: null, body: resultBody);
                    }
                }
            };

            _channel.BasicConsume(queue: "TransactionQueryQueue", autoAck: true, consumer: consumer);
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
*/

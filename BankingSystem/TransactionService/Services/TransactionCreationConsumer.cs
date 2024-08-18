using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TransactionService.Models;

namespace TransactionService.Services;

public class TransactionCreationConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IModel _channel;

    public TransactionCreationConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "transactionQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var transaction = JsonSerializer.Deserialize<Transaction>(message);

            if (transaction != null)
            {
                // Create a scope to use scoped services
                using (var scope = _serviceProvider.CreateScope())
                {
                    var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
                    await transactionService.CreateTransactionAsync(transaction.AccountId, transaction.Amount, transaction.Type);
                }
            }
        };

        _channel.BasicConsume(queue: "transactionQueue", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
    

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}

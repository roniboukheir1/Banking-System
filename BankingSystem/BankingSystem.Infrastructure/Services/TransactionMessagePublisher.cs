using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BankingSystem.Infrastructure.Services;

public class TransactionMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public TransactionMessagePublisher()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "transactionQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishTransaction(int accountId, decimal amount, string type)
    {
        var transaction = new
        {
            AccountId = accountId,
            Amount = amount,
            Type = type
        };

        var message = JsonSerializer.Serialize(transaction);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: "transactionQueue", basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}

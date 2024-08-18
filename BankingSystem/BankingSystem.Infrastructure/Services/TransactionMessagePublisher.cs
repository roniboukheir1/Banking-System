using System.Text;
using System.Text.Json;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class TransactionMessagePublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public TransactionMessagePublisher()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "transactionQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "TransactionResultQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
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

    public async Task<IEnumerable<Transaction>> PublishGetTransactionAsync(int accountId)
    {
        var request = new TransactionQueryMessage
        {
            AccountId = accountId
        };

        var message = JsonSerializer.Serialize(request);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: "TransactionQueryQueue", basicProperties: null, body: body);

        var tcs = new TaskCompletionSource<IEnumerable<Transaction>>();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var responseBody = ea.Body.ToArray();
            var responseMessage = Encoding.UTF8.GetString(responseBody);
            var transactionResult = JsonSerializer.Deserialize<TransactionResultMessage>(responseMessage);

            if (transactionResult != null)
            {
                tcs.SetResult(transactionResult.Transactions);
            }
            else
            {
                tcs.SetResult(Enumerable.Empty<Transaction>());
            }
        };

        _channel.BasicConsume(queue: "TransactionResultQueue", autoAck: true, consumer: consumer);

        return await tcs.Task;
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}

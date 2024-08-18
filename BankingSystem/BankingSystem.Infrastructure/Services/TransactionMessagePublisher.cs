using System.Text;
using System.Text.Json;
using BankingSystem.Domain.Models;
using BankingSystem.Infrastructure.Configuration;
using BankingSystem.Infrastructure.Messages;
using BankingSystem.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class TransactionMessagePublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IEmailService _emailService;
    private readonly TokenServices _tokenServices;
    private readonly ILogger<TransactionMessagePublisher> _logger;

    public TransactionMessagePublisher(IEmailService emailService, TokenServices tokenServices, ILogger<TransactionMessagePublisher> logger)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "transactionQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "TransactionResultQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _emailService = emailService;
        _tokenServices = tokenServices;
        _logger = logger;
    }

    public async void PublishTransaction(int accountId, decimal amount, string type)
    {
        try
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
            _logger.LogInformation("Published transaction message: {Message}", message);

            var userEmail = _tokenServices.GetUserEmailFromToken();
            _logger.LogInformation("Retrieved user email: {UserEmail}", userEmail);

            await _emailService.SendEmailAsync(userEmail, "Transaction Made",
                $"Dear Customer, a transaction was made: {message}");
            _logger.LogInformation("Sent email to {UserEmail} about the transaction", userEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while publishing transaction or sending email for AccountId: {AccountId}, Amount: {Amount}, Type: {Type}", accountId, amount, type);
        }
    }

    /*
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
    */

    public void Dispose()
    {
        try
        {
            _channel.Close();
            _connection.Close();
            _logger.LogInformation("RabbitMQ connection and channel closed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while closing RabbitMQ connection or channel.");
        }
    }
}

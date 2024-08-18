
using BankingSystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionMessagePublisher _messagePublisher;
    private readonly IMediator _mediator;

    public TransactionsController(TransactionMessagePublisher messagePublisher, IMediator mediator)
    {
        _mediator = mediator;
        _messagePublisher = messagePublisher;
    }

    [HttpPost]
    public IActionResult CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        _messagePublisher.PublishTransaction(request.AccountId, request.Amount, request.Type);
        return Ok("Transaction message sent");
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetCustomerTransactions([FromQuery] int accountId)
    {
        var query = new GetCustomerTransactions
        {
            AccountId = accountId
        };
        var transactions = _mediator.Send(query);
        return Ok(transactions);
    }
}

    public class CreateTransactionRequest
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
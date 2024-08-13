using BankingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers;

  [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionMessagePublisher _messagePublisher;

        public TransactionsController(TransactionMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        [HttpPost]
        public IActionResult CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            _messagePublisher.PublishTransaction(request.AccountId, request.Amount, request.Type);
            return Ok("Transaction message sent");
        }
    }

    public class CreateTransactionRequest
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
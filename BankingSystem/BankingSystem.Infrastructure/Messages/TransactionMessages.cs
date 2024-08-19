using BankingSystem.Domain.Models;

namespace BankingSystem.Infrastructure.Messages;
public class TransactionQueryMessage
{
    public int AccountId { get; set; }
}

public class TransactionResultMessage
{
    public IEnumerable<Transaction> Transactions { get; set; }
}

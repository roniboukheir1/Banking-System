namespace BankingSystem.Common.Exceptions;
using System;

public class TransactionsNotFoundException : Exception
{
    public TransactionsNotFoundException(int accountId)
        : base($"No transactions found for account ID {accountId}.")
    {
    }
}

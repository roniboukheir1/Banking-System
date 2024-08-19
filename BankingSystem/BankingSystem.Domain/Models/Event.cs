using System;
using System.Collections.Generic;

namespace BankingSystem.Domain.Models;


public class Event
{
    public int EventId { get; set; }
    public Guid AggregateId { get; set; }
    public virtual string EventType { get; set; } 
    public string EventData { get; set; } 
    public DateTime CreatedAt { get; set; }
}


public class TransactionCreatedEvent : Event
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } // Deposit, Withdrawal, etc.

    public override string EventType => "TransactionCreated";
}

public class TransactionRevertedEvent : Event
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }

    public override string EventType => "TransactionReverted";
}

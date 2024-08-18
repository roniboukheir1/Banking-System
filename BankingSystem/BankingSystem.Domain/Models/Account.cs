using System;
using System.Collections.Generic;

namespace BankingSystem.Domain.Models;

public class Account
{
    public int Id { get; set; }

    public string Accountnumber { get; set; } = null!;

    public int Customerid { get; set; }

    public int Branchid { get; set; }

    public decimal Balance { get; set; } = 0;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    private readonly List<Event> _changes = new List<Event>();
    
   public void Apply(TransactionCreatedEvent @event)
   {
      Balance += @event.Type == "Deposit" ? @event.Amount : -@event.Amount;
      _changes.Add(@event);
   }

   public void Apply(TransactionRevertedEvent @event)
   {
      Balance -= @event.Type == "Deposit" ? @event.Amount : -@event.Amount;
      _changes.Add(@event);
   }

   public IEnumerable<Event> GetUncommittedChanges()
   {
      return _changes;
   } 
}

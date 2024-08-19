using System;
using System.Collections.Generic;

namespace BankingSystem.Domain.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int Accountid { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public string Type { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}

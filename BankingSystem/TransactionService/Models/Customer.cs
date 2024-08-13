﻿namespace TransactionService.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

}

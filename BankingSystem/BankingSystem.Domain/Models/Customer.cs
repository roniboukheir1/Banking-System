using System;
using System.Collections.Generic;

namespace BankingSystem.Domain.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public Customer()
    {
    }

    public Customer(string name)
    {
        Name = name;
    }

    public Account CreateAccount(string accountNumber, int branchId)
    {
        Account account = new Account
        {
            Accountnumber = accountNumber,
            Balance = 0,
            Customerid = Id,
            Branchid = branchId,

        };
        Accounts.Add(account);
        return account;
    }

}

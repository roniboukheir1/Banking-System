namespace TransactionService.Models;
public class Account
{
    public int Id { get; set; }

    public string Accountnumber { get; set; } = null!;

    public int Customerid { get; set; }

    public int Branchid { get; set; }

    public decimal Balance { get; set; } = 0;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public void ApplyTransaction(Transaction transaction)
    {
       Balance += transaction.Type == "Deposit" ? transaction.Amount : -transaction.Amount;
    }
}

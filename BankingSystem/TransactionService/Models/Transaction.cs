namespace TransactionService.Models;

public class Transaction
{
    public int Id { get;  set; }
    public int AccountId { get; set; }
    public decimal Amount { get;set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }  // "Deposit" or "Withdrawal"
    public virtual Account Account { get; set; } = null!;
    

}
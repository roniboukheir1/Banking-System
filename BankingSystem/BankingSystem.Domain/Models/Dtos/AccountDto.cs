namespace BankingSystem.Persistance.Models.Dtos;

public class AccountDto
{
    public int AccountId { get; set; }
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
}
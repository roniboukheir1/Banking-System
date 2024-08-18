namespace BankingSystem.Persistance.Models.Dtos;

public class TenantDto
{
    public string TenantName { get; set; }
    public string TenantKey { get; set; }
    public string ConnectionString { get; set; }
}
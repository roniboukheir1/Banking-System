using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Models;

public class Tenant
{
    [Key]
    public Guid TenantId { get; set; }

    [Required]
    public byte[] EncryptedTenantKey { get; set; }

    [Required]
    [MaxLength(255)]
    public string TenantName { get; set; }

    [Required]
    [MaxLength(255)]
    public string ConnectionString { get; set; } 
}
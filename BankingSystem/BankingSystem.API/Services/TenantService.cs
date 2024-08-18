using BankingSystem.API.Services;
using BankingSystem.Domain.Models;
using BankingSystem.Persistance.Data;
using BankingSystem.Persistance.Models;

public class TenantService
{
    private readonly BankingSystemContext _context;

    public TenantService(BankingSystemContext context)
    {
        _context = context;
    }

    public async Task AddTenantAsync(string tenantName, string tenantKey, string connectionString)
    {
        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            TenantName = tenantName,
            EncryptedTenantKey = EncryptionHelper.Encrypt(tenantKey),
            ConnectionString =Convert.ToBase64String(EncryptionHelper.Encrypt(connectionString))  
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
    }
}

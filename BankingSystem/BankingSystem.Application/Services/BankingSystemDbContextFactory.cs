using BankingSystem.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Application.Services;


public class BankingSystemContextFactory
{
    private readonly DbContextOptions<BankingSystemContext> _options;
    private readonly string _defaultSchema;

    public BankingSystemContextFactory(DbContextOptions<BankingSystemContext> options, string defaultSchema)
    {
        _options = options;
        _defaultSchema = defaultSchema;
    }

    public DynamicBankingSystemContext CreateDbContext(string schema = null)
    {
        return new DynamicBankingSystemContext(_options, schema ?? _defaultSchema);
    }
}

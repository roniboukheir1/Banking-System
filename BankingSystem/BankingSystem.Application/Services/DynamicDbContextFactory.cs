
using System.Text;
using BankingSystem.API.Services;
using BankingSystem.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Application.Services
{
    public class DynamicDbContextFactory
    {
        private readonly DbContextOptions<BankingSystemContext> _options;

        public DynamicDbContextFactory(DbContextOptions<BankingSystemContext> options)
        {
            _options = options;
        }

        public async Task<DynamicBankingSystemContext> CreateDbContextAsync(string schema, string encryptedConnectionString, CancellationToken cancellationToken = default)
        {
            // Convert the encrypted connection string from a string to a byte array
            byte[] encryptedBytes = Encoding.UTF8.GetBytes(encryptedConnectionString);

            // Decrypt the connection string
            string connectionString = EncryptionHelper.Decrypt(encryptedBytes);

            var optionsBuilder = new DbContextOptionsBuilder<BankingSystemContext>(_options)
                .UseNpgsql(connectionString);

            return new DynamicBankingSystemContext(optionsBuilder.Options, schema);
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Application.Services
{
    public class MigrationService
    {
        private readonly DynamicDbContextFactory _contextFactory;

        public MigrationService(DynamicDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task ApplyMigrationsAsync(string schema, string encryptedConnectionString)
        {
            using var context = await _contextFactory.CreateDbContextAsync(schema, encryptedConnectionString);
            await context.Database.MigrateAsync();
        }
    }
}

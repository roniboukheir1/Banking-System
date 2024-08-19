using BankingSystem.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSystem.Persistance.Configurations;

public static class AddDatabaseConfiguration
{
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        
        {
            services.AddDbContext<BankingSystemContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")
                ));
            return services;
        }
}
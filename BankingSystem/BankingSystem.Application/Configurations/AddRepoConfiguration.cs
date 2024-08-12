using BankingSystem.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.CompilerServices;

namespace University_Management_System.Common.Configurations;

public static class AddRepoConfiguration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(Utils<>));
        return services;
    }
}

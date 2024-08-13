using BankingSystem.Application.Commands;
using BankingSystem.Application.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace University_Management_System.Common.Configurations;

public static class MediatRConfiguration
{
    public static IServiceCollection AddCqrsHandlers(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateAccountCommand).Assembly);
        services.AddMediatR(typeof(GetCustomerAccountsQuery).Assembly);

        return services;
    }
}
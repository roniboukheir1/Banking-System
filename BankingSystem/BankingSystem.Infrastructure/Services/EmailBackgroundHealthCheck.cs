using System.Net.Mail;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Infrastructure.Configuration;

public class EmailBackgroundJobHealthCheck : IHealthCheck
{
    private readonly IEmailService _emailService;

    public EmailBackgroundJobHealthCheck(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Implement logic to check the health of the email background job.

        try
        {
            _emailService.SendEmailAsync(
                "healthcheck@example.com",
                "Health Check",
                "This is a health check email."
            );
            return HealthCheckResult.Healthy("Email Service is healthy");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Email Service is unhealthy") ;
        }
    }
}

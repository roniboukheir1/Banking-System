using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace BankingSystem.Infrastructure.Configuration;
public static class EmailServiceExtensions
{
    public static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailService, EmailService>(provider =>
        {
            var smtpClient = new SmtpClient
            {
                Host = configuration["SmtpSettings:Server"],  
                Port = int.Parse(configuration["SmtpSettings:Port"]),  
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    configuration["SmtpSettings:Username"],  
                    configuration["SmtpSettings:Password"] 
                )
            };
            return new EmailService(smtpClient, configuration);
        });
    }
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly IConfiguration _configuration;

    public EmailService(SmtpClient smtpClient, IConfiguration configuration)
    {
        _smtpClient = smtpClient;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(to);

        await _smtpClient.SendMailAsync(mailMessage);
    }
}

/*
using BankingSystem.API.Services;
using BankingSystem.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, BankingSystemContext tenantDbContext)
    {
        string encryptedTenantKey = context.Request.Headers["X-Tenant-Key"];

        if (string.IsNullOrEmpty(encryptedTenantKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Tenant key is missing.");
            return;
        }

        var decryptedTenantKey = EncryptionHelper.Decrypt(Convert.FromBase64String(encryptedTenantKey));

        var tenant = await tenantDbContext.Tenants
            .FirstOrDefaultAsync(t => t.EncryptedTenantKey == EncryptionHelper.Encrypt(decryptedTenantKey));

        if (tenant == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Tenant not found.");
            return;
        }

        context.Items["TenantConnectionString"] = tenant.ConnectionString;

        await _next(context);
    }
}
*/

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace BankingSystem.Infrastructure.Configuration
{
    public static class AuthenticationConfiguration
    {
public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"{configuration["Keycloak:Authority"]}/realms/{configuration["Keycloak:Realm"]}";
            options.Audience = configuration["Keycloak:ClientId"];
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = $"{configuration["Keycloak:Authority"]}/realms/{configuration["Keycloak:Realm"]}",
                ValidAudience = configuration["Keycloak:ClientId"],
            };
        });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy =>
            policy.RequireRole("Admin"));
        options.AddPolicy("Employee", policy =>
            policy.RequireRole("Employee"));
        options.AddPolicy("Customer", policy =>
            policy.RequireRole("Customer"));
    });
}
        public static void UseAuthenticationServices(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

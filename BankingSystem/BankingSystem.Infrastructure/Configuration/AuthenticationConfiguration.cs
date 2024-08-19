
using System.Security.Claims;
using BankingSystem.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

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
                        RoleClaimType = "role" // Temporary role claim type
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                            if (claimsIdentity != null)
                            {
                                // Find the realm_access claim
                                var realmAccessClaim = claimsIdentity.FindFirst("realm_access");

                                if (realmAccessClaim != null && realmAccessClaim.Value != null)
                                {
                                    // Parse the realm_access claim to extract roles
                                    var roles = JObject.Parse(realmAccessClaim.Value)["roles"];

                                    if (roles != null)
                                    {
                                        foreach (var role in roles)
                                        {
                                            // Add each role as a new claim
                                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                                        }
                                    }
                                }
                            }

                            return System.Threading.Tasks.Task.CompletedTask;
                        }
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
                options.AddPolicy("BranchEmployeePolicy", policy =>
                    policy.Requirements.Add(new BranchEmployeeRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, BranchEmployeeHandler>();
        }
    }
}

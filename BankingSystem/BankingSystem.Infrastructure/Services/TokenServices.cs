using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BankingSystem.Infrastructure.Services;

public class TokenServices
{
   private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenServices(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserEmailFromToken()
    {
        var user = _httpContextAccessor.HttpContext.User;
        var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value;

        return emailClaim;
    }
}
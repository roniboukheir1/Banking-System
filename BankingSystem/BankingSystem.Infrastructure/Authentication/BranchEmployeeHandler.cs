using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BankingSystem.Infrastructure.Authorization;

namespace BankingSystem.Infrastructure.Authorization
{

    public class BranchEmployeeRequirement : IAuthorizationRequirement
    {
        public BranchEmployeeRequirement() { }
    }
}
    
    public class BranchEmployeeHandler : AuthorizationHandler<BranchEmployeeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchEmployeeRequirement requirement)
        {
            // Retrieve the branchId claim from the token
            var branchIdClaim = context.User.FindFirst("branchId")?.Value;
            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (branchIdClaim == null || roleClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            // If user is an Employee
            if (roleClaim == "employee")
            {
                // Extract the branchId from the resource
                var resource = context.Resource as Microsoft.AspNetCore.Http.HttpContext;
                var requestedBranchId = resource?.Request.RouteValues["branchId"]?.ToString();

                if (requestedBranchId == branchIdClaim)
                {
                    // Grant write access if they are accessing their branch
                    context.Succeed(requirement);
                }
                else
                {
                    // Grant only read access if they are accessing a different branch
                    if (resource?.Request.Method == Microsoft.AspNetCore.Http.HttpMethods.Get)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }

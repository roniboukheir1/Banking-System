using Microsoft.AspNetCore.Mvc;
using BankingSystem.Application.Repo;
using BankingSystem.Domain.Models;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/branch/{branchId}/[controller]")]
    public class BranchDataController : ControllerBase
    {
        private readonly BranchService _branchService;

        public BranchDataController(BranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetBranchCustomers(int branchId)
        {
            var customers = await _branchService.GetBranchAccountsAsync(branchId);
            return Ok(customers);
        }

        [HttpPost("customers")]
        public async Task<IActionResult> UpdateCustomer(int branchId, [FromBody] Account account)
        {
            var userClaims = User.Claims;
            await _branchService.UpdateAccountAsync(branchId, account);
            return Ok("Customer updated successfully.");
        }
    }
}

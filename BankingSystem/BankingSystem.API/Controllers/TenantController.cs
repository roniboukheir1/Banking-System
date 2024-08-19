using BankingSystem.Persistance.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers;


public class TenantsController : ControllerBase
{
    private readonly TenantService _tenantService;

    public TenantsController(TenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpPost("add-tenant")]
    [Authorize(Policy= "Admin")]
    public async Task<IActionResult> AddTenant([FromBody] TenantDto tenantDto)
    {
        if (tenantDto == null || string.IsNullOrEmpty(tenantDto.TenantName) || string.IsNullOrEmpty(tenantDto.TenantKey) || string.IsNullOrEmpty(tenantDto.ConnectionString))
        {
            return BadRequest("Invalid input parameters.");
        }

        await _tenantService.AddTenantAsync(tenantDto.TenantName, tenantDto.TenantKey, tenantDto.ConnectionString);
        return Ok("Tenant added successfully");
    }
}

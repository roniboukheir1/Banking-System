using BankingSystem.Application.Commands;
using BankingSystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
    {
        var accountId = await _mediator.Send(command);
        return Ok(new { AccountId = accountId });
    }

    [HttpGet("{customerId}/accounts")]
    public async Task<IActionResult> GetCustomerAccounts(int customerId)
    {
        var query = new GetCustomerAccountsQuery(customerId);
        var accounts = await _mediator.Send(query);
        return Ok(accounts);
    }
}

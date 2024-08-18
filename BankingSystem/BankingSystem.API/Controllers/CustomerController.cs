using BankingSystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid customer ID.");
        }

        var query = new GetCustomerAccountsQuery(id);
        var result = await _mediator.Send(query);

        if (result == null || !result.Any())
        {
            return NotFound($"No accounts found for customer ID {id}.");
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomer([FromBody] CreateCustomerCommand command)
    {
        if (command.Customer == null)
        {
            return BadRequest("Customer data is required.");
        }

        await _mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = command.Customer.Id }, command.Customer);
    }
}

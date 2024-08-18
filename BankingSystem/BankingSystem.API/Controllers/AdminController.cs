using BankingSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MediatR;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;

namespace BankingSystem.API.Controllers;

public class AdminController : ODataController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        return Ok(customers);
    }

    [EnableQuery]
    public async Task<IActionResult> Get([FromODataUri] int key)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(key));
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    public async Task<IActionResult> Post([FromBody] Customer customer)
    {
        await _mediator.Send(new CreateCustomerCommand(customer));
        return Created(customer);
    }

    public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] Customer update)
    {
        if (key != update.Id)
            return BadRequest();

        await _mediator.Send(new UpdateCustomerCommand(update));
        return Updated(update);
    }

    public async Task<IActionResult> Delete([FromODataUri] int key)
    {
        await _mediator.Send(new DeleteCustomerCommand(key));
        return NoContent();
    }
}

using BankingSystem.Application.Queries;
using BankingSystem.Domain.Models;
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

    [HttpGet("api/{id}")]
    public async Task<IActionResult>GetById(int id)
    {
        var query = new GetCustomerAccountsQuery(id);
        return Ok(await _mediator.Send(query));
    }

}

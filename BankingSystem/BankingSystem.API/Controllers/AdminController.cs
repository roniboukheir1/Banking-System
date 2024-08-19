using BankingSystem.Application.Commands;
using BankingSystem.Application.Queries;
using BankingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;


namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ODataController
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [EnableQuery]
        [HttpGet("customers")] 
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(customers);
        }

        [EnableQuery]
        [HttpGet("customersById")]
        public async Task<IActionResult> GetCustomerById([FromODataUri] int key)
        {
            var customer = await _mediator.Send(new GetCustomerByIdQuery(key));
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        [HttpPost("customers")] // POST api/admin/customers
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            await _mediator.Send(new CreateCustomerCommand(customer));
            return CreatedAtAction(nameof(GetCustomerById), new { key = customer.Id }, customer);
        }

        [HttpPut("customers")] // PUT api/admin/customers/{key}
        public async Task<IActionResult> UpdateCustomer([FromODataUri] int key, [FromBody] Customer update)
        {
            if (key != update.Id)
                return BadRequest();

            await _mediator.Send(new UpdateCustomerCommand(update));
            return Updated(update);
        }

        [HttpDelete("customers")] 
        public async Task<IActionResult> DeleteCustomer([FromODataUri] int key)
        {
            await _mediator.Send(new DeleteCustomerCommand(key));
            return NoContent();
        }
        
        [HttpPost("rollback-transactions")]
        public async Task<IActionResult> RollbackTransactions([FromBody] RollbackTransactionsCommand rollbackCommand)
        {
            if (rollbackCommand == null || rollbackCommand.Date == default)
            {
                return BadRequest("Invalid rollback request.");
            }

            await _mediator.Send(rollbackCommand);
            return Ok("Transactions rolled back successfully.");
        }
    }
}

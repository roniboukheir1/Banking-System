using BankingSystem.Application.Common;
using BankingSystem.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using University_Management_System.Common.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class CreateCustomerCommand : IRequest
{
    public Customer Customer { get; set; }
    public CreateCustomerCommand(Customer customer) => Customer = customer;
}

public class UpdateCustomerCommand : IRequest
{
    public Customer Customer { get; set; }
    public UpdateCustomerCommand(Customer customer) => Customer = customer;
}

public class DeleteCustomerCommand : IRequest
{
    public int Id { get; set; }
    public DeleteCustomerCommand(int id) => Id = id;
}


public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand>
{
    private readonly Utils<Customer> _utils;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(Utils<Customer> utils, ILogger<CreateCustomerCommandHandler> logger)
    {
        _utils = utils;
        _logger = logger;
    }

    public async Task Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateCustomerCommand for CustomerName: {CustomerName}", request.Customer.Name);
        await _utils.AddAsync(request.Customer);
        _logger.LogInformation("Customer created successfully with ID {CustomerId}", request.Customer.Id);
    }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly Utils<Customer> _utils;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(Utils<Customer> utils, ILogger<UpdateCustomerCommandHandler> logger)
    {
        _utils = utils;
        _logger = logger;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateCustomerCommand for CustomerId: {CustomerId}", request.Customer.Id);
        await _utils.UpdateAsync(request.Customer);
        _logger.LogInformation("Customer with ID {CustomerId} updated successfully.", request.Customer.Id);
    }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly Utils<Customer> _utils;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(Utils<Customer> utils, ILogger<DeleteCustomerCommandHandler> logger)
    {
        _utils = utils;
        _logger = logger;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteCustomerCommand for CustomerId: {CustomerId}", request.Id);
        await _utils.DeleteAsync(request.Id);
        _logger.LogInformation("Customer with ID {CustomerId} deleted successfully.", request.Id);
    }
}

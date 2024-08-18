using BankingSystem.Application.Common;
using BankingSystem.Domain.Models;
using MediatR;

public class GetAllCustomersQuery : IRequest<IEnumerable<Customer>> { }

public class GetCustomerByIdQuery : IRequest<Customer>
{
    public int Id { get; set; }
    public GetCustomerByIdQuery(int id) => Id = id;
}

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


public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<Customer>>
{
    private readonly Utils<Customer> _utils;
    public GetAllCustomersQueryHandler(Utils<Customer> utils)
    {
        _utils = utils;
    }

    public async Task<IEnumerable<Customer>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        return await _utils.GetAllAsync();
    }
}

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Customer>
{
    private readonly Utils<Customer> _utils;
    public GetCustomerByIdQueryHandler(Utils<Customer> utils)
    {
        _utils = utils;
    }

    public async Task<Customer> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        return await _utils.GetByIdAsync(request.Id);
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand>
{
    private readonly Utils<Customer> _utils;
    public CreateCustomerCommandHandler(Utils<Customer> utils)
    {
        _utils = utils;
    }

    public async Task Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        await _utils.AddAsync(request.Customer);
    }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly Utils<Customer> _utils;
    public UpdateCustomerCommandHandler(Utils<Customer> utils)
    {
        _utils = utils;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        await _utils.UpdateAsync(request.Customer);
    }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly Utils<Customer> _utils;
    public DeleteCustomerCommandHandler(Utils<Customer> utils)
    {
        _utils = utils;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        await _utils.DeleteAsync(request.Id);
    }
}

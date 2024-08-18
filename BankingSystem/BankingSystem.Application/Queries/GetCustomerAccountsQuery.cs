using BankingSystem.Application.Common;
using BankingSystem.Domain.Models;
using BankingSystem.Persistance.Data;
using BankingSystem.Persistance.Models;
using BankingSystem.Persistance.Models.Dtos;
using MediatR;
using University_Management_System.Common.Exceptions;

namespace BankingSystem.Application.Queries;

public class GetCustomerAccountsQuery : IRequest<IReadOnlyList<AccountDto>>
{
    public int CustomerId;
    public GetCustomerAccountsQuery(int customerid)
    {
        CustomerId = customerid;
    }
}
public class GetCustomerAccountsQueryHandler : IRequestHandler<GetCustomerAccountsQuery, IReadOnlyList<AccountDto>>
{
    private readonly Utils<Customer>_customerRepository;

    public GetCustomerAccountsQueryHandler(Utils<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<AccountDto>> Handle(GetCustomerAccountsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new NotFoundException("Customer not found.");
        }

        var accounts = customer.Accounts.Select(a => new AccountDto
        {
            AccountId = a.Id,
            AccountNumber = a.Accountnumber
        }).ToList();
        
        return accounts;
    }
}

    

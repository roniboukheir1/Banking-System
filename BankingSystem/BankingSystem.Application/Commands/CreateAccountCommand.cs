using BankingSystem.Application.Common;
using BankingSystem.Domain.Models;
using MediatR;
using University_Management_System.Common.Exceptions;

namespace BankingSystem.Application.Commands;


public class CreateAccountCommand : IRequest<int>
{
    public CreateAccountCommand(int customerId, string accountNumber)
    {
        CustomerId = customerId;
        AccountNumber = accountNumber;
    }

    public int CustomerId { get; }
    public string AccountNumber { get; }
}


public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, int>
{
   private readonly Utils<Customer> _customerRepository;

   public CreateAccountCommandHandler(Utils<Customer> customerRepository)
   {
       _customerRepository = customerRepository;
   } 
   public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new NotFoundException("Customer not found.");
        }

        var account = customer.CreateAccount(request.AccountNumber);

        return account.Id;
    }
}
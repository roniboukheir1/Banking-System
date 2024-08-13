using BankingSystem.Application.Common;
using BankingSystem.Domain.Models;
using MediatR;
using University_Management_System.Common.Exceptions;

namespace BankingSystem.Application.Commands;


public class CreateAccountCommand : IRequest<int>
{
    public CreateAccountCommand(int customerId,int branchid, string accountNumber)
    {
        CustomerId = customerId;
        BranchId = branchid;
        AccountNumber = accountNumber;
    }

    public int BranchId { get; }
    public int CustomerId { get; }
    public string AccountNumber { get; }
}


public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, int>
{
   private readonly Utils<Customer> _customerRepository;
   private readonly Utils<Account> _accountRepository;

   public CreateAccountCommandHandler(Utils<Customer> customerRepository, Utils<Account> accountRepo)
   {
       _accountRepository = accountRepo;
       _customerRepository = customerRepository;
   } 
   public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new NotFoundException("Customer not found.");
        } 
        var account = customer.CreateAccount(request.AccountNumber, request.BranchId);
        await _accountRepository.AddAsync(account);
        return account.Id;
    }
}
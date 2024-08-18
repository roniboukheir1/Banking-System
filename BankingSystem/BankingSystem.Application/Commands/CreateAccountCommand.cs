using BankingSystem.Application.Common;
using BankingSystem.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using University_Management_System.Common.Exceptions;

namespace BankingSystem.Application.Commands
{
    public class CreateAccountCommand : IRequest<int>
    {
        public CreateAccountCommand(int customerId, int branchId, string accountNumber)
        {
            CustomerId = customerId;
            BranchId = branchId;
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
        private readonly ILogger<CreateAccountCommandHandler> _logger;

        public CreateAccountCommandHandler(
            Utils<Customer> customerRepository,
            Utils<Account> accountRepository,
            ILogger<CreateAccountCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateAccountCommand for CustomerId: {CustomerId}, BranchId: {BranchId}, AccountNumber: {AccountNumber}", 
                                    request.CustomerId, request.BranchId, request.AccountNumber);

            // Check if the customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found.", request.CustomerId);
                throw new NotFoundException("Customer not found.");
            }

            // Check if the customer already has the maximum allowed number of accounts
            var customerAccounts = await _accountRepository.GetAllAsync();
            int accountCount = customerAccounts.Count(a => a.Customerid == request.CustomerId);
            if (accountCount >= 5)
            {
                _logger.LogWarning("Customer with ID {CustomerId} already has the maximum number of allowed accounts.", request.CustomerId);
                throw new InvalidOperationException("Customer has reached the maximum number of allowed accounts.");
            }

            // Create a new account for the customer
            var account = customer.CreateAccount(request.AccountNumber, request.BranchId);
            await _accountRepository.AddAsync(account);
            _logger.LogInformation("Created new account with ID {AccountId} for CustomerId: {CustomerId}", account.Id, request.CustomerId);

            return account.Id;
        }
    }
}

using BankingSystem.Application.Common;
using BankingSystem.Common.Exceptions;
using BankingSystem.Domain.Models;
using BankingSystem.Persistance.Data;
using BankingSystem.Persistance.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using University_Management_System.Common.Exceptions;

namespace BankingSystem.Application.Queries;

using System.Linq;

public class GetAllCustomersQuery : IRequest<IEnumerable<Customer>> { }

public class GetCustomerByIdQuery : IRequest<Customer>
{
    public int Id { get; set; }
    public GetCustomerByIdQuery(int id) => Id = id;
}

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<Customer>>
{
    private readonly Utils<Customer> _utils;
    private readonly ILogger<GetAllCustomersQueryHandler> _logger;

    public GetAllCustomersQueryHandler(Utils<Customer> utils, ILogger<GetAllCustomersQueryHandler> logger)
    {
        _utils = utils;
        _logger = logger;
    }

    public async Task<IEnumerable<Customer>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllCustomersQuery");
        return await _utils.GetAllAsync();
    }
}

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Customer>
{
    private readonly Utils<Customer> _utils;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(Utils<Customer> utils, ILogger<GetCustomerByIdQueryHandler> logger)
    {
        _utils = utils;
        _logger = logger;
    }

    public async Task<Customer> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCustomerByIdQuery for CustomerId: {CustomerId}", request.Id);
        var customer = await _utils.GetByIdAsync(request.Id);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found.", request.Id);
            throw new NotFoundException("Customer not found.");
        }
        return customer;
    }
}

public class GetCustomerAccountsQuery : IRequest<IReadOnlyList<AccountDto>>
{
    public int CustomerId { get; }

    public GetCustomerAccountsQuery(int customerId)
    {
        CustomerId = customerId;
    }
}

public class GetCustomerAccountsQueryHandler : IRequestHandler<GetCustomerAccountsQuery, IReadOnlyList<AccountDto>>
{
    private readonly Utils<Customer> _customerRepository;
    private readonly ILogger<GetCustomerAccountsQueryHandler> _logger;
    private readonly Utils<Account> _accountRepository;

    public GetCustomerAccountsQueryHandler(Utils<Customer> customerRepository, ILogger<GetCustomerAccountsQueryHandler> logger, Utils<Account> accountRepo)
    {
        _customerRepository = customerRepository;
        _logger = logger;
        _accountRepository = accountRepo;
    }

    public async Task<IReadOnlyList<AccountDto>> Handle(GetCustomerAccountsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCustomerAccountsQuery for CustomerId: {CustomerId}", request.CustomerId);

        if (request.CustomerId <= 0)
        {
            _logger.LogWarning("Invalid CustomerId: {CustomerId}", request.CustomerId);
            throw new ArgumentException("CustomerId must be a positive integer.");
        }

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found.", request.CustomerId);
            throw new NotFoundException("Customer not found.");
        }


        var accounts = await _accountRepository.GetAllAsync();

        IReadOnlyList<AccountDto> accountDtos = accounts
            .Select(account => new AccountDto
            {
                AccountId = account.Id,
                AccountNumber= account.Accountnumber,
                Balance = account.Balance
            })
        .ToList();

        _logger.LogInformation("Retrieved {AccountCount} accounts for CustomerId: {CustomerId}", accountDtos.Count, request.CustomerId);
        
        return accountDtos;
    }
}
public class GetCustomerTransactions : IRequest<IReadOnlyList<Transaction>>
{
    public int AccountId { get; set; }
    
    public GetCustomerTransactions(int accountId)
    {
        AccountId = accountId;
    }
}

public class GetCustomerTransactionsQueryHandler : IRequestHandler<GetCustomerTransactions, IReadOnlyList<Transaction>>
{
    private readonly BankingSystemContext _context;
    private readonly ILogger<GetCustomerTransactionsQueryHandler> _logger;

    public GetCustomerTransactionsQueryHandler(BankingSystemContext context, ILogger<GetCustomerTransactionsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Transaction>> Handle(GetCustomerTransactions request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCustomerTransactions for AccountId: {AccountId}", request.AccountId);

        if (request.AccountId <= 0)
        {
            _logger.LogWarning("Invalid AccountId: {AccountId}", request.AccountId);
            throw new ArgumentException("AccountId must be a positive integer.");
        }

        var transactions = await _context.Transactions
            .Where(t => t.Accountid == request.AccountId)
            .ToListAsync(cancellationToken);

        if (transactions == null || !transactions.Any())
        {
            _logger.LogWarning("No transactions found for AccountId: {AccountId}", request.AccountId);
            throw new TransactionsNotFoundException(request.AccountId);
        }

        _logger.LogInformation("{TransactionCount} transactions found for AccountId: {AccountId}", transactions.Count, request.AccountId);

        return transactions;
    }
}

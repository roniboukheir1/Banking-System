
using BankingSystem.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankingSystem.Common.Exceptions;
using BankingSystem.Persistance.Data;

namespace BankingSystem.Application.Queries
{
    public class GetCustomerTransactions : IRequest<IReadOnlyList<Transaction>>
    {
        public int AccountId { get; set; }
    }

    public class GetCustomerTransactionsQueryHandler : IRequestHandler<GetCustomerTransactions, IReadOnlyList<Transaction>>
    {
        private readonly BankingSystemContext _context;

        public GetCustomerTransactionsQueryHandler(BankingSystemContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Transaction>> Handle(GetCustomerTransactions request, CancellationToken cancellationToken)
        {
            var transactions = await _context.Transactions
                .Where(t => t.Accountid == request.AccountId)
                .ToListAsync(cancellationToken);
            if (transactions == null || !transactions.Any())
            {
                throw new TransactionsNotFoundException(request.AccountId);
            }

            return transactions;
        }
    }
}

using TransactionService.Data;
using TransactionService.Models;
using TransactionService.Repositories;

namespace TransactionService.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction> GetTransactionByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId);
        Task<int> CreateTransactionAsync(int accountId, decimal amount, string type);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly TransactionContext _context;

        public TransactionService(ITransactionRepository repository, TransactionContext context )
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _repository.GetAllTransactionsAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _repository.GetTransactionByIdAsync(id);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId)
        {
            return await _repository.GetTransactionsByAccountIdAsync(accountId);
        }

        public async Task<int> CreateTransactionAsync(int accountId, decimal amount, string type)
        {
           if (_context == null)
           {
               throw new InvalidOperationException("DbContext is not initialized.");
           } 
            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified), 
                Type = type
            };

            var account = await _context.Accounts.FindAsync(accountId);
            account.Balance += amount;
            _context.Update(account);
            await _context.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction.Id;
        }
    }
}

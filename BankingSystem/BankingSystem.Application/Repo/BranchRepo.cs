
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BankingSystem.Domain.Models;
using BankingSystem.Persistance.Data;

namespace BankingSystem.Application.Repo
{
    public class BranchService
    {

        private readonly BankingSystemContext _context;

        public BranchService(BankingSystemContext context)
        {
            _context = context;
        }
       public async Task SetSchemaAsync(string schema)
       {
           var sql = $"SET search_path TO {schema}";
           await _context.Database.ExecuteSqlRawAsync(sql);
       } 

        public async Task<IEnumerable<Account>> GetBranchAccountsAsync(int branchId)
        {
            var schema = $"branch{branchId}"; 
            await SetSchemaAsync(schema);

            return await _context.Accounts.ToListAsync();
        }

        public async Task UpdateAccountAsync(int branchId, Account account)
        {
            var schema = $"branch{branchId}";
            await SetSchemaAsync(schema);

            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.Id == account.Id);
            if (existingAccount != null)
            {
                await _context.AddAsync(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}
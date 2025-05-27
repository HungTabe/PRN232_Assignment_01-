using FUNewsManagementSystem.WebAPI.Data;
using FUNewsManagementSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly FUNewsManagementContext _context;

        public SystemAccountRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SystemAccount>> GetAllAsync()
        {
            return await _context.SystemAccounts.ToListAsync();
        }

        public async Task<SystemAccount> GetByIdAsync(short id)
        {
            return await _context.SystemAccounts.FindAsync(id);
        }

        public async Task AddAsync(SystemAccount account)
        {
            await _context.SystemAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SystemAccount account)
        {
            _context.SystemAccounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var account = await _context.SystemAccounts.FindAsync(id);
            if (account != null)
            {
                if (!await _context.NewsArticles.AnyAsync(na => na.CreatedById == id))
                {
                    _context.SystemAccounts.Remove(account);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Cannot delete account because it has associated news articles.");
                }
            }
        }

        public async Task<SystemAccount> AuthenticateAsync(string email, string password)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(sa => sa.AccountEmail == email && sa.AccountPassword == password);
        }

        public async Task<SystemAccount> RegisterAsync(SystemAccount account)
        {
            if (await _context.SystemAccounts.AnyAsync(sa => sa.AccountEmail == account.AccountEmail))
            {
                throw new Exception("Email already exists.");
            }

            var maxId = await _context.SystemAccounts.MaxAsync(sa => (short?)sa.AccountId) ?? 0;
            account.AccountId = (short)(maxId + 1);

            await _context.SystemAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }
    }
}

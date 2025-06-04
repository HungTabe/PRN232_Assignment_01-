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
            return await _context.SystemAccounts
                .AsNoTracking() // Tối ưu hiệu suất
                .ToListAsync();
        }

        public IQueryable<SystemAccount> Query()
        {
            return _context.SystemAccounts.AsQueryable();
        }

        public async Task<SystemAccount> GetByIdAsync(short id)
        {
            return await _context.SystemAccounts
                .AsNoTracking() // Tối ưu hiệu suất
                .FirstOrDefaultAsync(sa => sa.AccountId == id);
        }

        public async Task AddAsync(SystemAccount account)
        {
            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(account.AccountEmail))
            {
                throw new ArgumentException("Account email is required.");
            }
            if (await _context.SystemAccounts.AnyAsync(sa => sa.AccountEmail == account.AccountEmail))
            {
                throw new InvalidOperationException($"Email {account.AccountEmail} already exists.");
            }
            if (account.AccountRole < 0 || account.AccountRole > 2)
            {
                throw new ArgumentException("Invalid account role. Must be 0 (Admin), 1 (Staff), or 2 (Lecturer).");
            }

            // Gán AccountId tự động
            var maxId = await _context.SystemAccounts.MaxAsync(sa => (short?)sa.AccountId) ?? 0;
            account.AccountId = (short)(maxId + 1);

            await _context.SystemAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SystemAccount account)
        {
            var existing = await _context.SystemAccounts.FindAsync(account.AccountId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"System account with ID {account.AccountId} not found.");
            }
            if (string.IsNullOrWhiteSpace(account.AccountEmail))
            {
                throw new ArgumentException("Account email is required.");
            }
            if (account.AccountRole < 0 || account.AccountRole > 2)
            {
                throw new ArgumentException("Invalid account role. Must be 0 (Admin), 1 (Staff), or 2 (Lecturer).");
            }
            if (await _context.SystemAccounts.AnyAsync(sa => sa.AccountEmail == account.AccountEmail && sa.AccountId != account.AccountId))
            {
                throw new InvalidOperationException($"Email {account.AccountEmail} is already used by another account.");
            }

            _context.Entry(existing).CurrentValues.SetValues(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var account = await _context.SystemAccounts
                .Include(sa => sa.NewsArticles)
                .FirstOrDefaultAsync(sa => sa.AccountId == id);
            if (account == null)
            {
                throw new KeyNotFoundException($"System account with ID {id} not found.");
            }
            if (account.NewsArticles.Any())
            {
                throw new InvalidOperationException($"Cannot delete account {id} because it is associated with news articles.");
            }
            _context.SystemAccounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }
}

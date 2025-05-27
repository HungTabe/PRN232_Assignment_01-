using FUNewsManagementSystem.WebAPI.Models;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public interface ISystemAccountRepository
    {
        Task<IEnumerable<SystemAccount>> GetAllAsync();
        Task<SystemAccount> GetByIdAsync(short id);
        Task AddAsync(SystemAccount account);
        Task UpdateAsync(SystemAccount account);
        Task DeleteAsync(short id);
        Task<SystemAccount> AuthenticateAsync(string email, string password);
    }
}

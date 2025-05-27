using FUNewsManagementSystem.WebAPI.Models;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(short id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(short id);
    }
}

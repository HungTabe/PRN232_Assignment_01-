using FUNewsManagementSystem.WebAPI.Models;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag> GetByIdAsync(int id);
        Task AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(int id);
        IQueryable<Tag> Query();
    }
}

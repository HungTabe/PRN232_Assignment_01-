using FUNewsManagementSystem.WebAPI.Models;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public interface INewsArticleRepository
    {
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<NewsArticle> GetByIdAsync(string id);
        Task AddAsync(NewsArticle article);
        Task UpdateAsync(NewsArticle article);
        Task DeleteAsync(string id);
        Task<IEnumerable<NewsArticle>> GetByUserIdAsync(short userId);
        Task<IEnumerable<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate);
    }
}

using FUNewsManagementSystem.WebAPI.Data;
using FUNewsManagementSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly FUNewsManagementContext _context;

        public NewsArticleRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles
                .Include(na => na.Category)
                .Include(na => na.CreatedBy)
                .Include(na => na.Tags)
                .ToListAsync();
        }

        public async Task<NewsArticle> GetByIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(na => na.Category)
                .Include(na => na.CreatedBy)
                .Include(na => na.Tags)
                .FirstOrDefaultAsync(na => na.NewsArticleId == id);
        }

        public async Task AddAsync(NewsArticle article)
        {
            await _context.NewsArticles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NewsArticle article)
        {
            _context.NewsArticles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article != null)
            {
                _context.NewsArticles.Remove(article);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<NewsArticle>> GetByUserIdAsync(short userId)
        {
            return await _context.NewsArticles
                .Where(na => na.CreatedById == userId)
                .Include(na => na.Category)
                .Include(na => na.Tags)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.NewsArticles
                .Where(na => na.CreatedDate >= startDate && na.CreatedDate <= endDate)
                .OrderByDescending(na => na.CreatedDate)
                .Include(na => na.Category)
                .Include(na => na.CreatedBy)
                .ToListAsync();
        }
    }
}

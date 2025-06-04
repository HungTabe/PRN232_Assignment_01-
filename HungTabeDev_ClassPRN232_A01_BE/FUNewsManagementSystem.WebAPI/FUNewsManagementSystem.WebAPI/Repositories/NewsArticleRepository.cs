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
                .AsNoTracking()
                .ToListAsync();
        }

        public IQueryable<NewsArticle> Query()
        {
            return _context.NewsArticles
                .Include(na => na.Category)
                .Include(na => na.CreatedBy)
                .Include(na => na.Tags)
                .AsQueryable();
        }

        public async Task<NewsArticle> GetByIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(na => na.Category)
                .Include(na => na.CreatedBy)
                .Include(na => na.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(na => na.NewsArticleId == id);
        }

        public async Task AddAsync(NewsArticle article)
        {
            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(article.NewsArticleId))
            {
                throw new ArgumentException("News article ID is required.");
            }
            if (await _context.NewsArticles.AnyAsync(na => na.NewsArticleId == article.NewsArticleId))
            {
                throw new InvalidOperationException($"News article with ID {article.NewsArticleId} already exists.");
            }
            if (article.CategoryId.HasValue && !await _context.Categories.AnyAsync(c => c.CategoryId == article.CategoryId))
            {
                throw new ArgumentException($"Category ID {article.CategoryId} does not exist.");
            }
            if (article.CreatedById.HasValue && !await _context.SystemAccounts.AnyAsync(sa => sa.AccountId == article.CreatedById))
            {
                throw new ArgumentException($"CreatedBy ID {article.CreatedById} does not exist.");
            }

            // Liên kết với Tags hiện có, không tạo mới
            var existingTags = new List<Tag>();
            if (article.Tags != null && article.Tags.Any())
            {
                var tagIds = article.Tags.Select(t => t.TagId).ToList();
                existingTags = await _context.Tags
                    .Where(t => tagIds.Contains(t.TagId))
                    .ToListAsync();

                // Kiểm tra xem tất cả TagId có tồn tại không
                var missingTagIds = tagIds.Except(existingTags.Select(t => t.TagId)).ToList();
                if (missingTagIds.Any())
                {
                    throw new ArgumentException($"Tag IDs {string.Join(", ", missingTagIds)} do not exist.");
                }

                // Gán lại Tags để chỉ liên kết với các Tag hiện có
                article.Tags = existingTags;
            }

            await _context.NewsArticles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NewsArticle article)
        {
            var existing = await _context.NewsArticles
        .Include(na => na.Tags)
        .FirstOrDefaultAsync(na => na.NewsArticleId == article.NewsArticleId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"News article with ID {article.NewsArticleId} not found.");
            }
            if (article.CategoryId.HasValue && !await _context.Categories.AnyAsync(c => c.CategoryId == article.CategoryId))
            {
                throw new ArgumentException($"Category ID {article.CategoryId} does not exist.");
            }
            if (article.CreatedById.HasValue && !await _context.SystemAccounts.AnyAsync(sa => sa.AccountId == article.CreatedById))
            {
                throw new ArgumentException($"CreatedBy ID {article.CreatedById} does not exist.");
            }
            if (article.UpdatedById.HasValue && !await _context.SystemAccounts.AnyAsync(sa => sa.AccountId == article.UpdatedById))
            {
                throw new ArgumentException($"UpdatedBy ID {article.UpdatedById} does not exist.");
            }

            _context.Entry(existing).CurrentValues.SetValues(article);
            existing.CategoryId = article.CategoryId;
            existing.CreatedById = article.CreatedById;
            existing.UpdatedById = article.UpdatedById;
            existing.ModifiedDate = DateTime.UtcNow;

            existing.Tags.Clear();
            if (article.Tags != null && article.Tags.Any())
            {
                var tagIds = article.Tags.Select(t => t.TagId).ToList();
                var existingTags = await _context.Tags
                    .Where(t => tagIds.Contains(t.TagId))
                    .ToListAsync();

                var missingTagIds = tagIds.Except(existingTags.Select(t => t.TagId)).ToList();
                if (missingTagIds.Any())
                {
                    throw new ArgumentException($"Tag IDs {string.Join(", ", missingTagIds)} do not exist.");
                }

                foreach (var tag in existingTags)
                {
                    existing.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var article = await _context.NewsArticles
        .Include(na => na.Tags) 
        .FirstOrDefaultAsync(na => na.NewsArticleId == id);
            if (article == null)
            {
                throw new KeyNotFoundException($"News article with ID {id} not found.");
            }

            article.Tags.Clear();

            _context.NewsArticles.Remove(article);
            await _context.SaveChangesAsync();
        }

        public IQueryable<NewsArticle> QueryByUserId(short userId)
        {
            return _context.NewsArticles
                .Where(na => na.CreatedById == userId)
                .Include(na => na.Category)
                .Include(na => na.Tags)
                .AsQueryable();
        }

        public async Task<IEnumerable<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.NewsArticles
                .Where(na => na.CreatedDate >= startDate && na.CreatedDate <= endDate)
                .OrderByDescending(na => na.CreatedDate)
                .Include(na => na.Category)
                .Include(na => na.CreatedBy)
                .AsNoTracking() 
                .ToListAsync();
        }
    }
}

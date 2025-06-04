using FUNewsManagementSystem.WebAPI.Data;
using FUNewsManagementSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly FUNewsManagementContext _context;

        public TagRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .AsNoTracking() // Tối ưu hiệu suất
                .ToListAsync();
        }

        public IQueryable<Tag> Query()
        {
            return _context.Tags.AsQueryable();
        }

        public async Task<Tag> GetByIdAsync(int id)
        {
            return await _context.Tags
                .AsNoTracking() // Tối ưu hiệu suất
                .FirstOrDefaultAsync(t => t.TagId == id);
        }

        public async Task AddAsync(Tag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.TagName))
            {
                throw new ArgumentException("Tag name is required.");
            }
            if (await _context.Tags.AnyAsync(t => t.TagId == tag.TagId))
            {
                throw new InvalidOperationException($"Tag with ID {tag.TagId} already exists.");
            }
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            var existing = await _context.Tags.FindAsync(tag.TagId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Tag with ID {tag.TagId} not found.");
            }
            if (string.IsNullOrWhiteSpace(tag.TagName))
            {
                throw new ArgumentException("Tag name is required.");
            }
            _context.Entry(existing).CurrentValues.SetValues(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await _context.Tags
                .Include(t => t.NewsArticles) // Tải liên kết để kiểm tra
                .FirstOrDefaultAsync(t => t.TagId == id);
            if (tag == null)
            {
                throw new KeyNotFoundException($"Tag with ID {id} not found.");
            }
            if (tag.NewsArticles.Any())
            {
                throw new InvalidOperationException($"Cannot delete tag {id} because it is used by news articles.");
            }
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}

using FUNewsManagementSystem.WebAPI.Data;
using FUNewsManagementSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.WebAPI.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FUNewsManagementContext _context;

        public CategoryRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .ToListAsync();
        }

        public async Task<IQueryable<Category>> QueryAsync()
        {
            return _context.Categories;
        }

        public async Task<Category> GetByIdAsync(short id)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                if (!await _context.NewsArticles.AnyAsync(na => na.CategoryId == id))
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Cannot delete category because it is used by news articles.");
                }
            }
        }
    }
}

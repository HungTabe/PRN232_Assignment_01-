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
                .AsNoTracking()
                .ToListAsync();
        }

        public IQueryable<Category> Query()
        {
            return _context.Categories.AsQueryable();
        }

        public async Task<Category> GetByIdAsync(short id)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory) // Tải danh mục mẹ
                .AsNoTracking() // Tối ưu hiệu suất
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddAsync(Category category)
        {
            // Validate để tránh vòng lặp trong dữ liệu
            if (category.ParentCategoryId == category.CategoryId)
            {
                throw new ArgumentException("Category cannot be its own parent.");
            }
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            // Validate để tránh vòng lặp
            if (category.ParentCategoryId == category.CategoryId)
            {
                throw new ArgumentException("Category cannot be its own parent.");
            }
            var existing = await _context.Categories.FindAsync(category.CategoryId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Category with ID {category.CategoryId} not found.");
            }
            _context.Entry(existing).CurrentValues.SetValues(category);
            // Giữ ParentCategoryId để tránh cập nhật navigation property
            existing.ParentCategoryId = category.ParentCategoryId;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            if (await _context.NewsArticles.AnyAsync(na => na.CategoryId == id))
            {
                throw new InvalidOperationException("Cannot delete category because it is used by news articles.");
            }
            if (await _context.Categories.AnyAsync(c => c.ParentCategoryId == id))
            {
                throw new InvalidOperationException("Cannot delete category because it is a parent of other categories.");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}

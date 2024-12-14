using Microsoft.EntityFrameworkCore;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Data.Contexts;

namespace Pro_Web_API.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByCategoryNameAsync(string categoryName)
        {
            return await _context.Categories.FirstOrDefaultAsync(u => u.category_name == categoryName);
        }
    }
}

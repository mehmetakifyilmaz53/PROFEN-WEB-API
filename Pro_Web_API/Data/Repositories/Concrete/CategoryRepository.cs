using Microsoft.EntityFrameworkCore;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Data.Contexts;
using Pro_Web_API.Data.Repositories.Abstract;

namespace Pro_Web_API.Data.Repositories.Concrete
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
            try
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı ekleme işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<List<Category>> GetAllAsync()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı sorgulama işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<Category?> GetByCategoryNameAsync(string categoryName)
        {
            try
            {
                return await _context.Categories.FirstOrDefaultAsync(u => u.category_name == categoryName);
            }
            catch (DbUpdateException ex)
            {
                throw new AppException("Veritabanı sorgulama işlemi sırasında hata oluştu.", 500);
            }
        }
    }
}

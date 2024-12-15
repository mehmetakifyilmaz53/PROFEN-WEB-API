using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Data.Repositories.Abstract
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByCategoryNameAsync(string categoryName);
        Task<List<Category>> GetAllAsync();

        Task AddAsync(Category category);
    }
}

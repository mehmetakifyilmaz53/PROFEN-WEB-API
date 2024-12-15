using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Data.Repositories.Abstract
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByNameAsync(string productName);
        Task<List<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}

using MongoDB.Driver;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _collection;

        public ProductRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
            _collection = _database.GetCollection<Product>("Products");
        }

        public async Task AddAsync(Product product)
        {
            await _collection.InsertOneAsync(product);
        }

        public async Task DeleteAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Product?> GetByNameAsync(string productName)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}

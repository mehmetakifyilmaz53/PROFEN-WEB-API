using MongoDB.Bson;
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
            try
            {
                product.Id = await GetNextSequenceValue("productId");
                await _collection.InsertOneAsync(product);
            }
            catch (MongoException ex)
            {
                throw new AppException("Veritabanı işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task DeleteAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
                await _collection.DeleteOneAsync(filter);
            }
            catch (MongoException ex)
            {
                throw new AppException("Veritabanı silme işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<List<Product>> GetAllAsync()
        {
            try
            {
                return await _collection.Find(_ => true).ToListAsync();
            }
            catch (MongoException ex)
            {
                throw new AppException("Veritabanı sorgulama işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                throw new AppException("Veritabanı sorgulama işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task<Product?> GetByNameAsync(string productName)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Name, productName);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                throw new AppException("Veritabanı sorgulama işlemi sırasında hata oluştu.", 500);
            }
        }

        public async Task UpdateAsync(Product product)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
                await _collection.ReplaceOneAsync(filter, product);
            }
            catch (MongoException ex)
            {
                throw new AppException("Veritabanı güncelleme işlemi sırasında hata oluştu.", 500);
            }
        }

        private async Task<int> GetNextSequenceValue(string sequenceName)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", sequenceName);
                var update = Builders<BsonDocument>.Update.Inc("sequence_value", 1);

                var options = new FindOneAndUpdateOptions<BsonDocument>
                {
                    ReturnDocument = ReturnDocument.After, // Güncellenmiş belgeyi döner.
                    IsUpsert = true // Eğer kayıt yoksa oluşturur.
                };

                var result = await _collection.Database
                    .GetCollection<BsonDocument>("counters")
                    .FindOneAndUpdateAsync(filter, update, options);

                return result["sequence_value"].AsInt32;
            }
            catch (MongoException ex)
            {
                throw new AppException("Veritabanı sıralama işlemi sırasında hata oluştu.", 500);
            }
        }
    }
}

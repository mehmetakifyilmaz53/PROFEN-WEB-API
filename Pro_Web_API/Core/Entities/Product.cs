using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Pro_Web_API.Core.Entities
{
    public class Product
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
    }
}

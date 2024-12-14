namespace Pro_Web_API.Data.Contexts
{
    public class MongoDbContext
    {
        public class MongoDbSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }
    }
}

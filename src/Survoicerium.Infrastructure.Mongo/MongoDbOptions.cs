namespace Survoicerium.Infrastructure.Mongo
{
    public class MongoDbOptions
    {
        public string DbHost { get; set; }

        public string DbName { get; set; }

        public string CollectionName { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public MongoDbOptions(string dbHost, string dbName, string collectionName, string user, string password)
        {
            DbHost = dbHost;
            DbName = dbName;
            CollectionName = collectionName;
            User = user;
            Password = password;
        }
    }
}

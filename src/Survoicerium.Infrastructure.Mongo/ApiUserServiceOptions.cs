namespace Survoicerium.Infrastructure.Mongo
{
    public class ApiUserServiceOptions
    {
        public string DbHost { get; set; }

        public string DbName { get; set; }

        public string CollectionName { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public ApiUserServiceOptions(string dbHost, string dbName, string collectionName, string user, string password)
        {
            DbHost = dbHost;
            DbName = dbName;
            CollectionName = collectionName;
            User = user;
            Password = password;
        }
    }
}

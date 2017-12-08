using System;
using System.Collections.Generic;
using System.Text;

namespace Survoicerium.Infrastructure.Mongo
{
    public class ApiUserServiceOptions
    {
        public string ConnectionString { get; set; }

        public string DbName { get; set; }

        public string CollectionName { get; set; }

        public ApiUserServiceOptions(string connectionString, string dbName, string collectionName)
        {
            ConnectionString = connectionString;
            DbName = dbName;
            CollectionName = collectionName;
        }
    }
}

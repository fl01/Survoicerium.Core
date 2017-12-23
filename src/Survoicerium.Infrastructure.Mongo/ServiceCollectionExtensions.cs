using Microsoft.Extensions.DependencyInjection;
using Survoicerium.Core;

namespace Survoicerium.Infrastructure.Mongo
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApiService(this IServiceCollection services, string dbHost, string dbName, string collectionName, string dbUser, string dbPassword)
        {
            return services
                .AddScoped<IApiUserService>(builder => new ApiUserService(new ApiUserServiceOptions(dbHost, dbName, collectionName, dbUser, dbPassword)));
        }
    }
}

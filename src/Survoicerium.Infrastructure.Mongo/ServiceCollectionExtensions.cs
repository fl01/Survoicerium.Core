using Microsoft.Extensions.DependencyInjection;
using Survoicerium.Core;

namespace Survoicerium.Infrastructure.Mongo
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApiService(this IServiceCollection services, string connectionString)
        {
            return services
                .AddScoped<IApiUserService>(builder => new ApiUserService(connectionString));
        }
    }
}

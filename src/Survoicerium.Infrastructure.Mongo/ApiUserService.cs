using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Survoicerium.Core;
using Survoicerium.Core.Dto;

namespace Survoicerium.Infrastructure.Mongo
{
    public class ApiUserService : IApiUserService
    {
        private static Lazy<IMongoCollection<ApiUser>> _users = null;

        public ApiUserService(ApiUserServiceOptions options)
        {
            string connectionString;
            if (!string.IsNullOrEmpty(options.User) && !string.IsNullOrEmpty(options.Password))
            {
                connectionString = $"mongodb://{options.User}:{options.Password}@{options.DbHost}";
            }
            else
            {
                connectionString = $"mongodb://{options.DbHost}";
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(options.DbName);
            _users = new Lazy<IMongoCollection<ApiUser>>(() => database.GetCollection<ApiUser>(options.CollectionName));
        }

        public async Task<ApiUser> GetOrAddAsync(AddUserDto addUserDto)
        {
            var existing = await _users.Value.Find(u => u.Discord.UserId == addUserDto.DiscordUserId).SingleOrDefaultAsync();

            if (existing != null)
            {
                return existing;
            }

            var apiUser = new ApiUser()
            {
                ApiKey = Guid.NewGuid().ToString(),
                CreatedAt = 0,
                Discord = new DiscordAccount()
                {
                    UserId = addUserDto.DiscordUserId
                },
                HardwareIds =
                {
                    addUserDto.HardwareId
                },
                Id = Guid.NewGuid(),
                IsBanned = false
            };

            await _users.Value.InsertOneAsync(apiUser);

            return apiUser;
        }

        public async Task<ApiUser> GetUserByApiKeyAsync(string apiKey)
        {
            var existing = await _users.Value.Find(u => apiKey == u.ApiKey).FirstOrDefaultAsync();

            return existing;
        }
    }
}

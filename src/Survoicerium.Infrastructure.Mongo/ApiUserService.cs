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

        public ApiUserService(string connectionString)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("Survoicerium");
            _users = new Lazy<IMongoCollection<ApiUser>>(() => database.GetCollection<ApiUser>("Users"));
        }

        public async Task<IApiUser> GetOrAddAsync(AddUserDto addUserDto)
        {
            var existing = await _users.Value.Find(u => u.Discord.UserId == addUserDto.DiscordUserId).FirstOrDefaultAsync();

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

        public async Task<IApiUser> GetUserByHardwareIdAsync(string hardwareId)
        {
            var existing = await _users.Value.Find(u => u.HardwareIds.Contains(hardwareId)).FirstOrDefaultAsync();

            return existing;
        }

        public async Task<IApiUser> GetUserByApiKeyAsync(string apiKey)
        {
            var existing = await _users.Value.Find(u => string.Equals(apiKey, u.ApiKey, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();

            return existing;
        }

        public async Task<bool> IsValidApiKeyAsync(string apiKey)
        {
            return await _users.Value.Find(u => string.Equals(u.ApiKey, apiKey, StringComparison.OrdinalIgnoreCase)).AnyAsync();
        }
    }
}

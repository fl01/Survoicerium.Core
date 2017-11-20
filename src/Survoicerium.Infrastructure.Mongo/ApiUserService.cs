using Survoicerium.Core;
using Survoicerium.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survoicerium.Infrastructure.Mongo
{
    public class ApiUserService : IApiUserService
    {
        private readonly string _connectionString;

        // TODO : should be replaced with mongo
        private static Dictionary<Guid, ApiUser> _users = new Dictionary<Guid, ApiUser>();

        public ApiUserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IApiUser> AddAsync(AddUserDto addUserDto)
        {
            var existing = _users.FirstOrDefault(u => u.Value.Discord.UserId == addUserDto.DiscordUserId).Value;
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

            _users[apiUser.Id] = apiUser;

            return apiUser;
        }

        public Task<IApiUser> GetUserByHardwareIdAsync(string hardwareId)
        {
            var existing = _users.FirstOrDefault(u => u.Value.HardwareIds.Contains(hardwareId));

            return Task.FromResult<IApiUser>(existing.Value);
        }

        public Task<IApiUser> GetUserByApiKeyAsync(string apiKey)
        {
            var existing = _users.FirstOrDefault(u => string.Equals(apiKey, u.Value.ApiKey, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult<IApiUser>(existing.Value);
        }

        public bool IsValidApiKey(string apiKey)
        {
            return _users.Any(u => string.Equals(u.Value.ApiKey, apiKey, StringComparison.OrdinalIgnoreCase));
        }
    }
}

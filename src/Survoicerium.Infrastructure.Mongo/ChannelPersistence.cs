using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Survoicerium.Core;
using Survoicerium.Core.Abstractions;
using MongoModels = Survoicerium.Infrastructure.Mongo.Models;

namespace Survoicerium.Infrastructure.Mongo
{
    public class ChannelPersistence : IChannelPersistence
    {
        private Lazy<IMongoCollection<MongoModels.Channel>> _channels;

        public ChannelPersistence(MongoDbOptions options)
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
            _channels = new Lazy<IMongoCollection<MongoModels.Channel>>(() => database.GetCollection<MongoModels.Channel>(options.CollectionName));
        }

        public async Task DeleteChannelAsync(Guid channelId)
        {
            await _channels.Value.DeleteOneAsync(c => c._id == channelId);
        }

        public async Task<IEnumerable<Channel>> GetDiscordUserChannelsAsync(ulong discordUserId)
        {
            var cursor = await _channels.Value.FindAsync(c => c.Users.Any(u => u.Discord.UserId == discordUserId));
            var result = await cursor.ToListAsync();

            return AutoMapper.Mapper.Map<List<Models.Channel>, List<Channel>>(result);
        }

        public async Task<IEnumerable<Channel>> GetExpiredChannelsAsync(long currentTimeStamp)
        {
            var cursor = await _channels.Value.FindAsync(x => x.Expiry < currentTimeStamp);
            var result = await cursor.ToListAsync();

            return AutoMapper.Mapper.Map<List<Models.Channel>, List<Channel>>(result);
        }

        public async Task PersistChannelAsync(Channel channel)
        {
            var mongoChannel = AutoMapper.Mapper.Map<Channel, MongoModels.Channel>(channel);
            try
            {
                await _channels.Value.InsertOneAsync(mongoChannel);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // simply ignore
            }
        }

        public async Task PersistUserInChannelAsync(Guid id, ApiUser user)
        {
            Expression<Func<MongoModels.Channel, bool>> expression = x => x._id == id && !x.Users.Any(u => u.Id == user.Id);
            await _channels.Value.FindOneAndUpdateAsync(expression, Builders<MongoModels.Channel>.Update.Push(s => s.Users, user));
        }
    }
}

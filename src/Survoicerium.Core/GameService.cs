using Survoicerium.Core.Dto;
using Survoicerium.Core.Hash;
using Survoicerium.Messaging;
using Survoicerium.Messaging.Events;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Survoicerium.Core
{
    public class GameService : IGameService
    {
        private readonly IHashService _hashService;
        private readonly IEventBus _eventBus;
        private ConcurrentDictionary<string, Channel> _activeChannels = new ConcurrentDictionary<string, Channel>();

        public readonly TimeSpan _expiredChannelWatcherDelay = TimeSpan.FromMinutes(1);

        public GameService(IHashService hashService, IEventBus eventBus)
        {
            _hashService = hashService;
            _eventBus = eventBus;

            Task.Factory.StartNew(RunExpiredChannelWatcher, TaskCreationOptions.LongRunning);
        }

        public async Task JoinGameAsync(GameInfoDto game)
        {
            string channelId = _hashService.GetHash(game.Hash);
            var channel = new Channel()
            {
                Expiry = DateTimeOffset.UtcNow.AddMinutes(18).ToUnixTimeSeconds(),
                Id = channelId,
                Users = { game.User }
            };

            // channel already exists, simply add current user to a list
            if (!_activeChannels.TryAdd(channel.Id, channel))
            {
                _activeChannels[channel.Id].Users.Add(game.User);
            }

            var joinedGameEvent = new OnJoinedGameEvent()
            {
                ChannelId = channel.Id,
                VoiceUserId = game.User.Discord.UserId
            };

            await _eventBus.PublishAsync(joinedGameEvent);
        }

        private void RunExpiredChannelWatcher()
        {
            while (true)
            {
                Thread.Sleep(_expiredChannelWatcherDelay);

                long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var expiredChannels = _activeChannels.Where(k => k.Value.Expiry < currentTimestamp)
                    .Select(x => x.Key)
                    .ToList();

                foreach (string channelId in expiredChannels)
                {
                    // TODO : log & notify if failed to remove
                    _activeChannels.TryRemove(channelId, out Channel value);
                }
            }
        }
    }
}

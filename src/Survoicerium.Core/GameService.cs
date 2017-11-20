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
        private readonly INameService _hashService;
        private readonly IEventBus _eventBus;
        private ConcurrentDictionary<string, Channel> _activeChannels = new ConcurrentDictionary<string, Channel>();

        public readonly TimeSpan _expiredChannelWatcherDelay = TimeSpan.FromMinutes(1);

        public GameService(INameService hashService, IEventBus eventBus)
        {
            _hashService = hashService;
            _eventBus = eventBus;

            Task.Factory.StartNew(RunExpiredChannelWatcher, TaskCreationOptions.LongRunning);
        }

        public async Task JoinGameAsync(GameInfoDto game)
        {
            string channelName = _hashService.GetChannelName(game.Hash);
            var channel = new Channel()
            {
                Expiry = DateTimeOffset.UtcNow.AddMinutes(18).ToUnixTimeSeconds(),
                Name = channelName,
                Users = { game.User }
            };

            // channel already exists, simply add current user to a list
            if (!_activeChannels.TryAdd(channel.Name, channel))
            {
                _activeChannels[channel.Name].Users.Add(game.User);
            }

            var joinedGameEvent = new OnJoinedGameEvent()
            {
                ChannelName = channel.Name,
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

                foreach (string channelName in expiredChannels)
                {
                    // TODO : log & notify if failed to remove
                    _activeChannels.TryRemove(channelName, out Channel value);

                    _eventBus.PublishAsync(new OnChannelExpiredEvent() { ChannelName = channelName }).GetAwaiter().GetResult();
                }
            }
        }
    }
}

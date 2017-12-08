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
        private readonly TimeSpan _expiredChannelWatcherDelay = TimeSpan.FromSeconds(30);

        public GameService(INameService hashService, IEventBus eventBus)
        {
            _hashService = hashService;
            _eventBus = eventBus;

            Task.Factory.StartNew(RunExpiredChannelWatcher, TaskCreationOptions.LongRunning);
        }

        public async Task JoinGameAsync(GameInfoDto game)
        {
            string channelName = _hashService.GenerateChannelName(game.Hash);
            var channel = new Channel()
            {
                Expiry = GetChannelExpiry(),
                Name = channelName,
                Users = { game.User }
            };

            _activeChannels.AddOrUpdate(channel.Name, channel, (k, v) =>
            {
                v.Expiry = GetChannelExpiry();
                v.Users.Add(game.User);
                return v;
            });

            var joinedGameEvent = new OnJoinedGameEvent()
            {
                ChannelName = channel.Name,
                VoiceUserId = game.User.Discord.UserId
            };

            await _eventBus.PublishAsync(joinedGameEvent);
        }

        private long GetChannelExpiry()
        {
            return DateTimeOffset.UtcNow.AddMinutes(18).ToUnixTimeSeconds();
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

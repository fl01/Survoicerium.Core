using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Survoicerium.Core.Abstractions;
using Survoicerium.Core.Dto;
using Survoicerium.Core.Hash;
using Survoicerium.Messaging;
using Survoicerium.Messaging.Events;

namespace Survoicerium.Core
{
    public class GameService : IGameService
    {
        private readonly IHashService _hashService;
        private readonly IMessageBus _eventBus;
        private readonly IChannelPersistence _channelPersistence;
        private readonly TimeSpan _expiredChannelWatcherDelay = TimeSpan.FromSeconds(60);

        public GameService(IHashService hashService, IMessageBus eventBus, IMessageChannel eventChannel, IChannelPersistence channelPersistence)
        {
            _hashService = hashService;
            _eventBus = eventBus;
            _channelPersistence = channelPersistence;

            // TODO : this should be moved to a separate worker, but for now it is fine
            eventChannel.On<OnChatRequestToJoinVoiceChannel>(HandleOnChatRequestToJoinVoiceChannel);
            eventChannel.Start();

            Task.Factory.StartNew(RunExpiredChannelWatcher, TaskCreationOptions.LongRunning);
        }

        public async Task JoinGameAsync(GameInfoDto game)
        {
            (Guid Id, string Name) channelIdentifier = _hashService.GenerateChannelIdentifier(game.Hash);

            var channel = new Channel()
            {
                Id = channelIdentifier.Id,
                Expiry = GetChannelExpiry(),
                Name = channelIdentifier.Name
            };

            await _channelPersistence.PersistChannelAsync(channel);
            await _channelPersistence.PersistUserInChannelAsync(channel.Id, game.User);

            await SendOnJoinedGameEvent(channel.Name, game.User.Discord.UserId);
        }

        private async void HandleOnChatRequestToJoinVoiceChannel(object args)
        {
            var request = args as OnChatRequestToJoinVoiceChannel;

            var userChannels = await _channelPersistence.GetDiscordUserChannelsAsync(request.UserId);
            var lastActiveChannel = userChannels.OrderByDescending(x => x.Expiry).FirstOrDefault();
            if (lastActiveChannel != null)
            {
                await SendOnJoinedGameEvent(lastActiveChannel.Name, request.UserId);
            }
        }

        private async Task SendOnJoinedGameEvent(string channelName, ulong voiceUserId)
        {
            var joinedGameEvent = new OnJoinedGameEvent()
            {
                ChannelName = channelName,
                VoiceUserId = voiceUserId
            };

            await _eventBus.PublishAsync(joinedGameEvent);
        }

        private long GetChannelExpiry()
        {
            return DateTimeOffset.UtcNow.AddMinutes(45).ToUnixTimeSeconds();
        }

        private async void RunExpiredChannelWatcher()
        {
            while (true)
            {
                Thread.Sleep(_expiredChannelWatcherDelay);
                var expiredChannels = await _channelPersistence.GetExpiredChannelsAsync(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                foreach (Channel channel in expiredChannels)
                {
                    // TODO : log & notify if failed to remove
                    await _channelPersistence.DeleteChannelAsync(channel.Id);
                    await _eventBus.PublishAsync(new OnChannelExpiredEvent() { ChannelName = channel.Name });
                }
            }
        }
    }
}

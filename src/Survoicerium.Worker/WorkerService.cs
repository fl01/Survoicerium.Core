using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Survoicerium.Core;
using Survoicerium.Core.Abstractions;
using Survoicerium.Messaging;
using Survoicerium.Messaging.Messages.Events;

namespace Survoicerium.Worker
{
    public class WorkerService
    {
        private readonly IMessageChannel _messageChannel;
        private readonly IMessageBus _messageBus;
        private readonly IChannelPersistence _channelPersistence;
        private readonly TimeSpan _expiredChannelWatcherDelay = TimeSpan.FromSeconds(60);

        public WorkerService(IMessageChannel messageChannel, IMessageBus messageBus, IChannelPersistence channelPersistence)
        {
            _messageChannel = messageChannel;
            _messageBus = messageBus;
            _channelPersistence = channelPersistence;

            _messageChannel.On<OnChatRequestToJoinVoiceChannel>(HandleOnChatRequestToJoinVoiceChannel);
        }

        public void Start()
        {
            Task.Factory.StartNew(RunExpiredChannelWatcher, TaskCreationOptions.LongRunning);

            _messageChannel.Start();
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
                    await _messageBus.PublishAsync(new OnChannelExpiredEvent() { ChannelName = channel.Name });
                }
            }
        }

        private async void HandleOnChatRequestToJoinVoiceChannel(object args)
        {
            var request = args as OnChatRequestToJoinVoiceChannel;

            var userChannels = await _channelPersistence.GetDiscordUserChannelsAsync(request.UserId);
            var lastActiveChannel = userChannels.OrderByDescending(x => x.Expiry).FirstOrDefault();
            if (lastActiveChannel != null)
            {
                var joinedGameEvent = new OnJoinedGameEvent()
                {
                    ChannelName = lastActiveChannel.Name,
                    VoiceUserId = request.UserId
                };

                await _messageBus.PublishAsync(joinedGameEvent);
            }
        }
    }
}

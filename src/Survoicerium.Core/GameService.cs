using System;
using System.Threading.Tasks;
using Survoicerium.Core.Abstractions;
using Survoicerium.Core.Dto;
using Survoicerium.Core.Models;
using Survoicerium.Messaging;
using Survoicerium.Messaging.Messages.Events;

namespace Survoicerium.Core
{
    public class GameService : IGameService
    {
        private readonly IHashService _hashService;
        private readonly IMessageBus _messageBus;
        private readonly IChannelPersistence _channelPersistence;

        public GameService(IHashService hashService, IMessageBus messageBus, IChannelPersistence channelPersistence)
        {
            _hashService = hashService;
            _messageBus = messageBus;
            _channelPersistence = channelPersistence;
        }

        public async Task JoinGameAsync(GameInfoDto game)
        {
            ChannelIdentifier channelIdentifier = _hashService.GenerateChannelIdentifier(game.Hash);

            var channel = new Channel()
            {
                Id = channelIdentifier.Id,
                Expiry = DateTimeOffset.UtcNow.AddMinutes(45).ToUnixTimeSeconds(),
                Name = channelIdentifier.Name
            };

            await _channelPersistence.PersistChannelAsync(channel);
            await _channelPersistence.PersistUserInChannelAsync(channel.Id, game.User);

            var joinedGameEvent = new OnJoinedGameEvent()
            {
                ChannelName = channel.Name,
                VoiceUserId = game.User.Discord.UserId
            };

            await _messageBus.PublishAsync(joinedGameEvent);
        }
    }
}

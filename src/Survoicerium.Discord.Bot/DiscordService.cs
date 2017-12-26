using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Survoicerium.Messaging;
using Survoicerium.Messaging.Events;

namespace Survoicerium.Discord.Bot
{
    public class DiscordService
    {
        private DiscordSocketClient _client;
        private readonly string _token;
        private readonly IMessageBus _eventBus;
        private readonly object _getChannelLock = new object();

        public const ulong LogsTextChannelId = 389023517003218954;
        public const ulong WaitingRoomVoiceChannelId = 388770842911178762;
        public const ulong DefaultGuildId = 370680552334032897;

        private SocketTextChannel LogsTextChannel { get; set; }
        private SocketGuild DefaultServer { get; set; }

        public DiscordService(string token, Messaging.IMessageChannel messageChannel, IMessageBus messageBus)
        {
            _token = token;
            _eventBus = messageBus;

            messageChannel.On<OnJoinedGameEvent>(x => HandleIfReady<OnJoinedGameEvent>(x, OnJoinedGameEventReceived));
            messageChannel.On<OnChannelExpiredEvent>(x => HandleIfReady<OnChannelExpiredEvent>(x, OnChannelExpiredReceived));
        }

        public async Task ConnectAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.Connected += () =>
            {
                DefaultServer = _client.Guilds.FirstOrDefault(s => s.Id == DefaultGuildId);
                var start = DateTime.UtcNow;
                while ((LogsTextChannel = DefaultServer.GetTextChannel(LogsTextChannelId)) == null)
                {
                    if ((DateTime.UtcNow - start).TotalMinutes >= 5)
                    {
                        // TODO : logs or notify somehow
                        Console.WriteLine("Failed to initialize after 5 minutes");
                        break;
                    }

                    Thread.Sleep(1000);
                }

                return Task.CompletedTask;
            };

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
        }

        private async void OnJoinedGameEventReceived(OnJoinedGameEvent args)
        {
            await LogsTextChannel.SendMessageAsync($"Received join game '{args.ChannelName}' for player '{args.VoiceUserId}'");

            ulong channelId = 0;
            lock (_getChannelLock)
            {
                channelId = DefaultServer.VoiceChannels.FirstOrDefault(v => string.Equals(v.Name, args.ChannelName, StringComparison.OrdinalIgnoreCase))?.Id
                    ?? DefaultServer.CreateVoiceChannelAsync(args.ChannelName).GetAwaiter().GetResult().Id;
            }

            var user = DefaultServer.GetUser(args.VoiceUserId);
            if (user != null)
            {
                await user.ModifyAsync(x => x.ChannelId = channelId);
                await LogsTextChannel.SendMessageAsync($"User '{user.Username}' has been moved to '{args.ChannelName}' with id '{channelId}'");
            }
        }

        private async void OnChannelExpiredReceived(OnChannelExpiredEvent args)
        {
            var channel = DefaultServer.VoiceChannels.FirstOrDefault(v => string.Equals(args.ChannelName, v.Name, StringComparison.OrdinalIgnoreCase));
            if (channel != null)
            {
                foreach (var user in channel.Users.ToList())
                {
                    await user.ModifyAsync(x => x.ChannelId = WaitingRoomVoiceChannelId);
                }

                await channel.DeleteAsync();
                await LogsTextChannel.SendMessageAsync($"Channel '{args.ChannelName}' has been deleted due to expiry");
            }
        }

        private void HandleIfReady<T>(object item, Action<T> handler)
            where T : class
        {
            if (_client.LoginState != LoginState.LoggedIn)
            {
                return;
            }

            handler(item as T);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            // TODO: command parser etc
            if (message.Content == "!join")
            {
                await HandleChatRequestToJoinVoiceChannel(message);
            }
        }

        private async Task HandleChatRequestToJoinVoiceChannel(SocketMessage message)
        {
            bool isUserInWaitingRoom = (DefaultServer.GetVoiceChannel(WaitingRoomVoiceChannelId)?.Users?.Any(u => u.Id == message.Author.Id)).GetValueOrDefault();
            if (!isUserInWaitingRoom)
            {
                await message.Channel.SendMessageAsync("Please join 'WaitingRoom' and try again");
            }
            else
            {
                await _eventBus.PublishAsync(new OnChatRequestToJoinVoiceChannel() { UserId = message.Author.Id });
                await message.Channel.SendMessageAsync("Your request has been accepted. You will be moved to voice channel in case you have an active game");
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

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
        private readonly IEventBus _eventBus;
        private readonly object _getChannelLock = new object();

        public const ulong DefaultGuildTextChannelId = 370681552679469056;
        public const ulong DefaultGuildVoiceChannelId = 388770842911178762;
        public const ulong DefaultGuildId = 370680552334032897;

        private SocketTextChannel DefaultTextChannel { get; set; }
        private SocketGuild DefaultServer { get; set; }

        public DiscordService(string token, IEventChannel eventChannel, IEventBus eventBus)
        {
            _token = token;
            _eventBus = eventBus;

            eventChannel.On<OnJoinedGameEvent>(x => HandleIfReady<OnJoinedGameEvent>(x, OnJoinedGameEventReceived));
            eventChannel.On<OnChannelExpiredEvent>(x => HandleIfReady<OnChannelExpiredEvent>(x, OnChannelExpiredReceived));
        }

        public async Task ConnectAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.Connected += () =>
            {
                DefaultServer = _client.Guilds.FirstOrDefault(s => s.Id == DefaultGuildId);

                Debug.Assert(DefaultServer != null);
                // TODO: could cause a dead lock, add retries
                while ((DefaultTextChannel = DefaultServer.GetTextChannel(DefaultGuildTextChannelId)) == null)
                {
                    Thread.Sleep(1000);
                }

                return Task.CompletedTask;
            };

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
        }

        private async void OnJoinedGameEventReceived(OnJoinedGameEvent args)
        {
            await DefaultTextChannel.SendMessageAsync($"Received join game '{args.ChannelName}' for player '{args.VoiceUserId}'");

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
                await DefaultTextChannel.SendMessageAsync($"User '{user.Username}' has been moved to '{args.ChannelName}' with id '{channelId}'");
            }
        }

        private async void OnChannelExpiredReceived(OnChannelExpiredEvent args)
        {
            var channel = DefaultServer.VoiceChannels.FirstOrDefault(v => string.Equals(args.ChannelName, v.Name, StringComparison.OrdinalIgnoreCase));
            if (channel != null)
            {
                foreach (var user in channel.Users.ToList())
                {
                    await user.ModifyAsync(x => x.ChannelId = DefaultGuildVoiceChannelId);
                }

                await channel.DeleteAsync();
                await DefaultTextChannel.SendMessageAsync($"Channel '{args.ChannelName}' has been deleted due to expiry");
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
            bool isUserInWaitingRoom = (DefaultServer.GetVoiceChannel(DefaultGuildVoiceChannelId)?.Users?.Any(u => u.Id == message.Author.Id)).GetValueOrDefault();
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

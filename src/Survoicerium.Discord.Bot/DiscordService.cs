using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Survoicerium.Discord.Bot.ApiClient;
using Survoicerium.Messaging;
using Survoicerium.Messaging.Events;

namespace Survoicerium.Discord.Bot
{
    public class DiscordService
    {
        private DiscordSocketClient _client;
        private readonly string _token;
        private readonly BackendApiClient _apiClient;

        public const ulong DefaultDiscordChannel = 370681552679469056;
        public const ulong DefaultServerId = 370680552334032897;

        private SocketTextChannel DefaultTextChannel { get; set; }
        private SocketGuild DefaultServer { get; set; }

        public DiscordService(string token, BackendApiClient apiClient, IEventChannel eventChannel)
        {
            _token = token;
            _apiClient = apiClient;

            eventChannel.On<PingEvent>(x => HandleIfReady((PingEvent)x, OnPingEventReceived));
            eventChannel.On<OnJoinedGameEvent>(x => HandleIfReady((OnJoinedGameEvent)x, OnJoinedGameEventReceived));
        }

        public async Task ConnectAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.Connected += () =>
            {
                DefaultServer = _client.Guilds.FirstOrDefault(s => s.Id == DefaultServerId);
                Debug.Assert(DefaultServer != null);
                DefaultTextChannel = DefaultServer.GetTextChannel(DefaultDiscordChannel);
                return Task.CompletedTask;
            };

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
        }

        private async void OnJoinedGameEventReceived(OnJoinedGameEvent args)
        {
            await DefaultTextChannel.SendMessageAsync($"Received join game '{args.GameHash}' for player '{args.UserId}'");

            if (!DefaultServer.VoiceChannels.Any(v => string.Equals(v.Name, args.GameHash)))
            {
                var channel = await DefaultServer.CreateVoiceChannelAsync(args.GameHash);
            }
        }

        private async void OnPingEventReceived(PingEvent args)
        {
            await DefaultTextChannel.SendMessageAsync($"Received: '{args.Message}'");
        }

        private void HandleIfReady<T>(T item, Action<T> handler)
        {
            if (_client.LoginState != LoginState.LoggedIn)
            {
                return;
            }

            handler(item);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

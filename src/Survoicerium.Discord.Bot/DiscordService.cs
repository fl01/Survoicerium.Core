using System;
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

        public DiscordService(string token, BackendApiClient apiClient, IEventChannel eventChannel)
        {
            _token = token;
            _apiClient = apiClient;

            eventChannel.On<PingEvent>(OnPingEventReceived);
        }

        private async void OnPingEventReceived(object args)
        {
            if (_client.LoginState != LoginState.LoggedIn)
            {
                return;
            }

            // TODO : should be cached etc...
            var guestChannel = _client.Guilds.FirstOrDefault(s => s.Id == DefaultServerId).GetTextChannel(DefaultDiscordChannel);
            await guestChannel?.SendMessageAsync($"Received: '{(args as PingEvent).Message}'");
        }

        public async Task ConnectAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
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

using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Survoicerium.Discord.Bot
{
    public class EntryPoint
    {
        private DiscordSocketClient _client;

        public static void Main(string[] args)
            => new EntryPoint().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var svc = new DiscordService("MzcwOTg4NTM1NDI1MjY5NzYy.DMvJCQ.ZTz5a5kRdOtMshSXROJrPiAb-Ec");
            await svc.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}

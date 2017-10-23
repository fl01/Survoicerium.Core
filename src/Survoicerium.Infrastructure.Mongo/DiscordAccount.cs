using System;
using Survoicerium.Core;

namespace Survoicerium.Infrastructure.Mongo
{
    public class DiscordAccount : IDiscordAccount
    {
        public ulong UserId { get; set; }
    }
}

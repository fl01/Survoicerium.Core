using System;
using System.Collections.Generic;
using Survoicerium.Core;

namespace Survoicerium.Infrastructure.Mongo
{
    public class ApiUser : IApiUser
    {
        public Guid Id { get; set; }

        public string ApiKey { get; set; }

        public HashSet<string> HardwareIds { get; set; } = new HashSet<string>();

        public IDiscordAccount Discord { get; set; }

        public int CreatedAt { get; set; }

        public bool IsBanned { get; set; }
    }
}

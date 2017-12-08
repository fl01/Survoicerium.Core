using System;
using System.Collections.Generic;

namespace Survoicerium.Core
{
    public class ApiUser
    {
        public Guid Id { get; set; }

        public string ApiKey { get; set; }

        public HashSet<string> HardwareIds { get; set; } = new HashSet<string>();

        public DiscordAccount Discord { get; set; }

        public long CreatedAt { get; set; }

        public bool IsBanned { get; set; }
    }
}

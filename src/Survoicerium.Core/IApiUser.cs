using System;
using System.Collections.Generic;

namespace Survoicerium.Core
{
    public interface IApiUser
    {
        Guid Id { get; set; }

        string ApiKey { get; set; }

        HashSet<string> HardwareIds { get; }

        IDiscordAccount Discord { get; set; }

        long CreatedAt { get; set; }

        bool IsBanned { get; set; }
    }
}

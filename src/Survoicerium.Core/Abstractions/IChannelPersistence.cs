using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Survoicerium.Core.Abstractions
{
    public interface IChannelPersistence
    {
        Task PersistChannelAsync(Channel channel);

        Task DeleteChannelAsync(Guid id);

        Task<IEnumerable<Channel>> GetExpiredChannelsAsync(long currentTimeStamp);

        Task PersistUserInChannelAsync(Guid id, ApiUser user);

        Task<IEnumerable<Channel>> GetDiscordUserChannelsAsync(ulong discordUserId);
    }
}

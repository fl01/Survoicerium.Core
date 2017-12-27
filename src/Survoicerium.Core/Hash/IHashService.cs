using System;

namespace Survoicerium.Core.Hash
{
    public interface IHashService
    {
        (Guid, string) GenerateChannelIdentifier(string channelId);
    }
}

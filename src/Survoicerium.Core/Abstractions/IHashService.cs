using System;
using Survoicerium.Core.Models;

namespace Survoicerium.Core.Abstractions
{
    public interface IHashService
    {
        ChannelIdentifier GenerateChannelIdentifier(string channelId);
    }
}

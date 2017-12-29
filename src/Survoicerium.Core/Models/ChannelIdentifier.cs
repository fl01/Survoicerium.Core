using System;

namespace Survoicerium.Core.Models
{
    public class ChannelIdentifier
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ChannelIdentifier(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

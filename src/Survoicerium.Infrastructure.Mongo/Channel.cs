using Survoicerium.Core;
using System.Collections.Generic;

namespace Survoicerium.Infrastructure.Mongo
{
    public class Channel
    {
        public long Expiry { get; set; }

        public string Id { get; set; }

        public ICollection<IApiUser> Users { get; set; } = new List<IApiUser>();
    }
}

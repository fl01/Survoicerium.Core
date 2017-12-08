using Survoicerium.Core;
using System.Collections.Generic;

namespace Survoicerium.Core
{
    public class Channel
    {
        public long Expiry { get; set; }

        public string Name { get; set; }

        public ICollection<ApiUser> Users { get; set; } = new List<ApiUser>();
    }
}

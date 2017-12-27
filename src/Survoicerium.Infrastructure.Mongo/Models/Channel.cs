using System;
using System.Collections.Generic;
using Survoicerium.Core;

namespace Survoicerium.Infrastructure.Mongo.Models
{
    public class Channel
    {
        public Guid _id;

        public long Expiry { get; set; }

        public string Name { get; set; }

        public IList<ApiUser> Users { get; set; } = new List<ApiUser>();
    }
}

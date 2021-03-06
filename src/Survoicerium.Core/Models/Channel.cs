﻿using Survoicerium.Core;
using System;
using System.Collections.Generic;

namespace Survoicerium.Core
{
    public class Channel
    {
        public Guid Id { get; set; }

        public long Expiry { get; set; }

        public string Name { get; set; }

        public IList<ApiUser> Users { get; set; } = new List<ApiUser>();
    }
}

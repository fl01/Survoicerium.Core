using System;

namespace Survoicerium.Messaging.Events
{
    public class OnJoinedGameEvent : Event
    {
        public long UserId { get; set; }

        public string GameHash { get; set; }

        public TimeSpan RoomLifetime { get; set; }
    }
}

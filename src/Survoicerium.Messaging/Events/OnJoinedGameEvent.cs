﻿namespace Survoicerium.Messaging.Events
{
    public class OnJoinedGameEvent : Event
    {
        public ulong VoiceUserId { get; set; }

        public string ChannelName { get; set; }
    }
}

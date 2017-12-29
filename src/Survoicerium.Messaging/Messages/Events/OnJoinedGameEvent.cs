namespace Survoicerium.Messaging.Messages.Events
{
    /// <summary>
    /// TODO: command ?
    /// </summary>
    public class OnJoinedGameEvent : Event
    {
        public ulong VoiceUserId { get; set; }

        public string ChannelName { get; set; }
    }
}

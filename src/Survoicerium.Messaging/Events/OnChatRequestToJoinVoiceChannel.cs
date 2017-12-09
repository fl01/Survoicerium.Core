namespace Survoicerium.Messaging.Events
{
    public class OnChatRequestToJoinVoiceChannel : Event
    {
        public ulong UserId { get; set; }
    }
}

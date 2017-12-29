namespace Survoicerium.Messaging.Messages.Events
{
    public class OnChatRequestToJoinVoiceChannel : Event
    {
        public ulong UserId { get; set; }
    }
}

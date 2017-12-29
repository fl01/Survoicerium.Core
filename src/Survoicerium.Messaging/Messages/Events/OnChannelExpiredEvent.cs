namespace Survoicerium.Messaging.Messages.Events
{
    public class OnChannelExpiredEvent : Event
    {
        public string ChannelName { get; set; }
    }
}

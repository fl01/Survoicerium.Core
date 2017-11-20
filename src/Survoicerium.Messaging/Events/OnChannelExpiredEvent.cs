namespace Survoicerium.Messaging.Events
{
    public class OnChannelExpiredEvent : Event
    {
        public string ChannelName { get; set; }
    }
}

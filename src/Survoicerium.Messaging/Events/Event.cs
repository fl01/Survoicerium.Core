namespace Survoicerium.Messaging.Events
{
    public abstract class Event : IEvent
    {
        public string SourceId { get; set; }
    }
}

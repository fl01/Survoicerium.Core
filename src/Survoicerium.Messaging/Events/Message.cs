namespace Survoicerium.Messaging.Events
{
    public abstract class Message
    {
        public string SourceId { get; set; }

        public string CorrelationId { get; set; }
    }
}

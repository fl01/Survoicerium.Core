namespace Survoicerium.Messaging.Messages
{
    public abstract class Message
    {
        public string SourceId { get; set; }

        public string CorrelationId { get; set; }
    }
}

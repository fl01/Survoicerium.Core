using System;

namespace Survoicerium.Messaging.Serialization
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object graph);

        T Deserialize<T>(byte[] data);

        object Deserialize(byte[] data, Type type);
    }
}

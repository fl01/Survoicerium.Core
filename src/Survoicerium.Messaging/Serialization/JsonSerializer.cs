using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Survoicerium.Messaging.Serialization
{
    public class JsonSerializer : IMessageSerializer
    {
        public byte[] Serialize(object graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            string json = JsonConvert.SerializeObject(graph);
            return Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            try
            {
                string json = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException e)
            {
                throw new SerializationException(e.Message, e);
            }
        }

        public object Deserialize(byte[] data, Type type)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            try
            {
                string json = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject(json, type);
            }
            catch (JsonSerializationException e)
            {
                throw new SerializationException(e.Message, e);
            }
        }
    }
}

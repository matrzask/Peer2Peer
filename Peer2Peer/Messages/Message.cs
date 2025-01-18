using System.Text.Json;
using System.Text.Json.Serialization;
using Peer2Peer.Nodes;
namespace Peer2Peer.Messages
{

    [JsonConverter(typeof(MessageConverter))]
    public abstract class Message
    {
        public string Type => GetType().Name;
        public Node Sender { get; set; }
        public string Payload { get; set; }

        [JsonConstructor]
        public Message(Node sender, string payload)
        {
            Sender = sender;
            Payload = payload;
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Message Deserialize(string json)
        {
            var message = JsonSerializer.Deserialize<Message>(json);
            if (message == null)
            {
                throw new InvalidOperationException("Deserialization resulted in a null Message object.");
            }
            return message;
        }

        public abstract void Execute(Node receiver);
    }

}
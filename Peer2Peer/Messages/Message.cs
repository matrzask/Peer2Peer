using System.Text.Json;
using Peer2Peer.Nodes;
namespace Peer2Peer.Messages
{
    /*public enum MessageType
    {
        WorkCompleted,
        RequestWork,
        AssignWork,
        PasswordFound
    }*/

    public abstract class Message
    {
        public Node Sender { get; set; }
        public string Payload { get; set; }

        public Message(Node sender, string payload)
        {
            Sender = sender;
            Payload = payload;
        }

        public Message() { }

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
using System.Text.Json;

public enum MessageType
{
    WorkCompleted,
    RequestWork,
    AssignWork,
    PasswordFound
}

public class Message
{
    public Node Sender { get; set; }
    public string Payload { get; set; }
    public MessageType Type { get; set; }

    public Message(Node sender, string payload, MessageType type)
    {
        Sender = sender;
        Payload = payload;
        Type = type;
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
}
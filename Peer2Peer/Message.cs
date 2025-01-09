using Microsoft.AspNetCore.SignalR;

internal class Message
{
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Payload { get; set; }

    public Message(string senderId, string receiverId, string payload)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Payload = payload;
    }

    public void Serialize()
    {
        throw new NotImplementedException();
    }

    public static Message Deserialize()
    {
        throw new NotImplementedException();
    }
}
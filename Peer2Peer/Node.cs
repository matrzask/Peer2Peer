public class Node
{
    readonly string nodeId;

    protected void SendMessenge(Message message, Node node)
    {
        node.ReceiveMessage(message);
    }


    public virtual void ReceiveMessage(Message message) { }
}
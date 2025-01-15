using Peer2Peer.Messages;

namespace Peer2Peer.Nodes
{
    public abstract class Node
    {
        public string nodeId;

        protected void SendMessage(Message message, Node node)
        {
            message.Execute(node);
        }
    }
}
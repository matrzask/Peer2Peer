using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    public class NewNodeMessage : Message
    {
        public NewNodeMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            node.AddNode(Sender);
        }
    }
}
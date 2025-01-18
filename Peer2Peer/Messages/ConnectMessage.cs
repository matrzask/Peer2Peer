using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    public class ConnectMessage : Message
    {
        public ConnectMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            node.SendNodeRegistry(Sender);
        }
    }
}
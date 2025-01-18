using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    public class RequestWorkMessage : Message
    {
        public RequestWorkMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            if (node is CoordinatorNode coordinator)
            {
                coordinator.HandleWorkerNode(Sender);
            }
        }
    }
}
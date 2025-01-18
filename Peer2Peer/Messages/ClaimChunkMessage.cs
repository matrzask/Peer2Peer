using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    public class ClaimChunkMessage : Message
    {
        public ClaimChunkMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            if (node is WorkerNode worker)
            {
                worker.HandleChunkClaim(Payload, Sender);
            }
        }
    }
}
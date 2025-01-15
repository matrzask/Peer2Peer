using Peer2Peer.Helpers;
using Peer2Peer.Nodes;
using System.Text.Json;

namespace Peer2Peer.Messages
{
    public class WorkCompletedMessage : Message
    {
        public WorkCompletedMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            WorkChunk chunk = JsonSerializer.Deserialize<WorkChunk>(Payload);
            if (chunk != null)
            {
                node.WorkCompleted(chunk, Sender.nodeId);
            }
        }
    }
}
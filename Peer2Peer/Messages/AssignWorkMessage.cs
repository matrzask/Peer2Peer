using Peer2Peer.Helpers;
using Peer2Peer.Nodes;
using System.Text.Json;

namespace Peer2Peer.Messages
{
    public class AssignWorkMessage : Message
    {
        public AssignWorkMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            if (node is WorkerNode worker)
            {
                WorkChunk chunk = JsonSerializer.Deserialize<WorkChunk>(Payload);
                worker.StartWork(chunk);
            }
        }
    }
}
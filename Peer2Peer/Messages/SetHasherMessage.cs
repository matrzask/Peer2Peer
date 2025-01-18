using System.Text.Json;
using Peer2Peer.Helpers;
using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    class SetHasherMessage : Message
    {
        public SetHasherMessage(Node sender, string payload) : base(sender, payload)
        {
        }

        public override void Execute(Node node)
        {
            if (node is WorkerNode worker)
            {
                var hasher = JsonSerializer.Deserialize<Hasher>(Payload);
                if (hasher == null)
                {
                    throw new InvalidOperationException("Deserialization resulted in a null Hasher object.");
                }
                worker.SetHasher(hasher);
            }
        }
    }
}
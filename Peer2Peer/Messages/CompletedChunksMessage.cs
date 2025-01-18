using System.Text.Json;
using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    class CompletedChunksMessage : Message
    {
        public CompletedChunksMessage(Node sender, string payload) : base(sender, payload) { }
        public override void Execute(Node receiver)
        {
            List<string> chunks = JsonSerializer.Deserialize<List<string>>(Payload);
            if (chunks != null)
            {
                receiver.SetCompletedChunks(chunks);
            }
        }
    }
}